# FIAP.Payments

API de gerenciamento de pagamentos desenvolvida como parte do projeto FIAP.

## 📋 Descrição

Microserviço responsável pelo processamento de pagamentos, integração com outros serviços e gerenciamento de transações financeiras.

## 🏗️ Arquitetura

Este projeto segue uma arquitetura em camadas:

- **Payments.Api**: Camada de apresentação (Controllers, Program.cs, Queue Workers)
- **Payments.Domain**: Camada de domínio (Entidades, Serviços, Interfaces, Message Bus)
- **Payments.Infrastructure**: Camada de infraestrutura (Repositórios, Migrations, Banco de dados)
- **Payments.Domain.Shared**: DTOs e contratos compartilhados entre serviços

## 🚀 Tecnologias

- .NET (C#)
- Entity Framework Core
- MassTransit (Message Bus)
- RabbitMQ
- Prometheus (Métricas)
- Grafana (Visualização)
- Docker
- Kubernetes (K8s) na AWS

## ☸️ Infraestrutura

Esta aplicação é implantada em um **cluster Kubernetes (K8s) na AWS**, utilizando:

- ConfigMaps para configurações
- Secrets para informações sensíveis
- Deployments para orquestração de containers
- Services para descoberta e balanceamento de carga
- HPA (Horizontal Pod Autoscaler) para escalonamento automático
- Prometheus para coleta de métricas
- Grafana para visualização e dashboards de monitoramento
- RabbitMQ para comunicação assíncrona entre microserviços

Os arquivos de configuração do Kubernetes estão localizados na pasta `kubernetes/`.

## 📦 Build e Deploy

O projeto possui configuração de CI/CD através de GitHub Actions para deploy automático no Amazon ECR e Kubernetes.

## 🔧 Requisitos

- .NET SDK
- Docker (para containerização)
- RabbitMQ (para message bus)
- Acesso ao cluster Kubernetes na AWS (para deploy)

## 📝 Licença

Este projeto faz parte do projeto acadêmico FIAP.
