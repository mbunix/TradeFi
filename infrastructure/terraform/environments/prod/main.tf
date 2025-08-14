

terraform {
  required_version = ">= 1.10.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.20"
    }
    helm = {
      source  = "hashicorp/helm"
      version = "~> 2.10"
    }
    kubectl = {
      source  = "hashicorp/kubectl"
      version = "~> 1.14"
    }
  }
    backend "s3" {
    bucket  = "tradefi-terraform-state-prod"
    key = "prod/terraform.tfstate"
    region = "af-south-1"
    encrypt = true
    dynamodb_table = "tradefi-terraform-locks-prod"
    }
    provider "aws" {
    region = "var.aws_region"
    default_tags {
      tags = {
        Environment = var.environment
        Project     = "TradeFi"
        ManagedBy   = "Terraform"
        Owner       = "TradeFi-Devops"
      }
    }
    data "aws_availability_zones" "available" {
      state = "available"
    }
    data "aws_caller_identity" "current" {}
    locals {
        cluster_name = "tradefi-${var.environment}-eks-cluster"
        common_tags = {
          Environment = var.environment
          Project     = "TradeFi"
          ManagedBy   = "Terraform"
          Owner       = "TradeFi-Devops"
        }
    }
}  
