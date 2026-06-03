provider "aws" {
  region = "ap-south-1"
}

resource "aws_db_instance" "postgres" {

  allocated_storage = 20

  engine = "postgres"

  engine_version = "15"

  instance_class = "db.t3.micro"

  db_name = "assignmentdb"

  username = "postgres"

  password = "postgres123"

  skip_final_snapshot = true
}

resource "aws_ecr_repository" "repo" {
  name = "assignment-api"
}