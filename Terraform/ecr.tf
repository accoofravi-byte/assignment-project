resource "aws_ecr_repository" "api" {
  name                 = "assignment-api"
  image_tag_mutability = "MUTABLE"

  force_delete = true
}

resource "aws_ecr_repository" "frontend" {
  name         = "assignment-ui"
  force_delete = true
}