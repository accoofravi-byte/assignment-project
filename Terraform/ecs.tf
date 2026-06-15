resource "aws_cloudwatch_log_group" "ecs" {
  name              = "/ecs/assignment"
  retention_in_days = 7
}

resource "aws_ecs_cluster" "main" {
  name = "assignment-cluster"
}

resource "aws_ecs_task_definition" "api" {
  family                   = "assignment-task"
  requires_compatibilities = ["FARGATE"]

  network_mode = "awsvpc"

  cpu    = 512
  memory = 1024

  execution_role_arn = aws_iam_role.ecs_execution_role.arn

  container_definitions = jsonencode([
    {
      name  = "assignment-api"
      image = "${aws_ecr_repository.api.repository_url}:latest"

      essential = true

      environment = [
        {
          name  = "ConnectionStrings__DefaultConnection"
          value = "Host=${aws_db_instance.postgres.address};Port=5432;Database=assignmentdb;Username=${var.db_username};Password=${var.db_password}"
        }
      ]

      portMappings = [
        {
          containerPort = 8080
          hostPort      = 8080
        }
      ]

      logConfiguration = {
        logDriver = "awslogs"

        options = {
          awslogs-group         = aws_cloudwatch_log_group.ecs.name
          awslogs-region        = var.aws_region
          awslogs-stream-prefix = "ecs"
        }
      }
    }
  ])
}

resource "aws_ecs_service" "api" {
  name            = "assignment-api"
  cluster         = aws_ecs_cluster.main.id
  task_definition = aws_ecs_task_definition.api.arn

  desired_count = 1
  launch_type   = "FARGATE"

  network_configuration {
    subnets = [
      aws_subnet.public_1.id,
      aws_subnet.public_2.id
    ]

    security_groups = [
      aws_security_group.ecs_sg.id
    ]

    assign_public_ip = true
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.api.arn
    container_name   = "assignment-api"
    container_port   = 8080
  }

  depends_on = [
    aws_lb_listener.http
  ]
}