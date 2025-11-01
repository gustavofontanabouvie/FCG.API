# FCG.API - API de Catálogo de Jogos

## 📋 Sobre o Projeto

FCG.API é uma API RESTful desenvolvida em .NET 8.0 para gerenciamento de um catálogo de jogos com sistema de promoções. A aplicação permite o cadastro de usuários, autenticação, gerenciamento de jogos e suas promoções, oferecendo uma solução completa para plataformas de distribuição digital de jogos.

### Objetivos

- Fornecer uma API robusta e escalável para gerenciamento de catálogos de jogos
- Implementar sistema de autenticação e autorização seguro com JWT
- Permitir gerenciamento de promoções com diferentes tipos de desconto
- Oferecer uma arquitetura limpa e modular baseada em camadas
- Facilitar a integração com sistemas frontend e mobile

## 🚀 Tecnologias Utilizadas

- **Framework**: .NET 8.0
- **Linguagem**: C#
- **Banco de Dados**: PostgreSQL
- **ORM**: Entity Framework Core 9.0
- **Autenticação**: JWT Bearer Authentication
- **Documentação**: Swagger/OpenAPI
- **Containerização**: Docker & Docker Compose
- **Testes**: xUnit, Moq, FluentAssertions
- **Validação**: FluentValidation
- **Criptografia**: BCrypt.Net
- **Logging**: Serilog

## 📁 Estrutura do Projeto

```
FCG.API/
├── FCG.API/                    # Camada de apresentação (API)
│   ├── Controllers/            # Controladores da API
│   ├── Properties/             # Configurações do projeto
│   ├── appsettings.json        # Configurações da aplicação
│   └── Program.cs              # Ponto de entrada da aplicação
├── Application/                # Camada de aplicação
│   ├── DTOs/                   # Data Transfer Objects
│   ├── Interfaces/             # Interfaces de serviços
│   └── Services/               # Implementação dos serviços
├── Fcg.Domain/                 # Camada de domínio
│   ├── Common/                 # Entidades comuns
│   ├── Game.cs                 # Entidade de jogo
│   ├── Promotion.cs            # Entidade de promoção
│   ├── PurchasedGame.cs        # Entidade de jogos comprados
│   └── User.cs                 # Entidade de usuário
├── Fcg.Data/                   # Camada de acesso a dados
│   ├── Context/                # Contexto do EF Core
│   └── Repositories/           # Implementação dos repositórios
├── Fcg.Infra/                  # Camada de infraestrutura
├── Fcg.Shared/                 # Recursos compartilhados
├── Fcg.Tests/                  # Testes unitários
└── docker-compose.yml          # Configuração do Docker Compose
```

## 📦 Pré-requisitos

Antes de começar, você precisará ter instalado em sua máquina:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) ou superior
- [PostgreSQL 13+](https://www.postgresql.org/download/) (ou use Docker)
- [Docker](https://www.docker.com/get-started) e [Docker Compose](https://docs.docker.com/compose/install/) (opcional, mas recomendado)
- [Git](https://git-scm.com/)
- Um editor de código (recomendado: [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/))

## ⚙️ Instalação e Configuração

### 1. Clone o repositório

```bash
git clone https://github.com/gustavofontanabouvie/FCG.API.git
cd FCG.API
```

### 2. Configuração do Banco de Dados

#### Opção A: Usando Docker (Recomendado)

O projeto já inclui um arquivo `docker-compose.yml` configurado. Para iniciar o banco de dados:

```bash
docker-compose up -d db
```

Isso iniciará um container PostgreSQL com:
- **Host**: localhost
- **Porta**: 5432
- **Database**: FcgDataBase
- **Usuário**: postgres
- **Senha**: postgres

#### Opção B: PostgreSQL Local

Se preferir usar uma instalação local do PostgreSQL, crie um banco de dados:

```sql
CREATE DATABASE FcgDataBase;
```

Depois, atualize a string de conexão no arquivo `FCG.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=FcgDataBase;Username=seu_usuario;Password=sua_senha"
  }
}
```

### 3. Restaurar Dependências

```bash
dotnet restore
```

### 4. Aplicar Migrações do Banco de Dados

As migrações são aplicadas automaticamente na inicialização da aplicação, mas você também pode aplicá-las manualmente:

```bash
cd FCG.API
dotnet ef database update
```

## 🏃 Executando a Aplicação

### Modo Desenvolvimento (Local)

1. Navegue até a pasta do projeto da API:

```bash
cd FCG.API
```

2. Execute a aplicação:

```bash
dotnet run
```

3. A API estará disponível em:
   - HTTP: `http://localhost:5000`
   - HTTPS: `https://localhost:5001`
   - Swagger: `https://localhost:5001/swagger`

### Usando Docker Compose (Recomendado para Produção)

Execute toda a stack (API + Banco de Dados + Adminer):

```bash
docker-compose up --build
```

A aplicação estará disponível em:
- **API**: `http://localhost:8080`
- **Swagger**: `http://localhost:8080/swagger`
- **Adminer** (Gerenciador de BD): `http://localhost:8081`

Para parar os containers:

```bash
docker-compose down
```

## 🧪 Executando os Testes

### Executar todos os testes

```bash
dotnet test
```

### Executar testes com cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Executar testes de um projeto específico

```bash
dotnet test Fcg.Tests/Fcg.Tests.csproj
```

## 📖 Documentação da API

A API utiliza Swagger para documentação interativa. Após iniciar a aplicação, acesse:

```
https://localhost:5001/swagger
```

### Endpoints Principais

#### 🔐 Autenticação

**POST** `/api/Auth/login`
- Autentica um usuário e retorna um token JWT
- **Body**:
```json
{
  "email": "usuario@exemplo.com",
  "password": "senha123"
}
```
- **Resposta**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### 👤 Usuários

**POST** `/api/User`
- Cria um novo usuário
- **Body**:
```json
{
  "name": "João Silva",
  "email": "joao@exemplo.com",
  "password": "Senha@123"
}
```

**GET** `/api/User/{id}`
- Busca um usuário por ID
- **Requer**: Token JWT

**PATCH** `/api/User/{id}`
- Atualiza dados de um usuário
- **Requer**: Token JWT com role Admin

**POST** `/api/User/Admin`
- Cria um novo usuário administrador
- **Requer**: Token JWT com role Admin

#### 🎮 Jogos

**POST** `/api/Game`
- Cadastra um novo jogo
- **Requer**: Token JWT com role Admin
- **Body**:
```json
{
  "name": "The Witcher 3",
  "description": "RPG de ação em mundo aberto",
  "price": 99.90,
  "releaseDate": "2015-05-19"
}
```

**GET** `/api/Game/{id}`
- Busca um jogo por ID
- **Requer**: Token JWT

**GET** `/api/Game/with-promotion`
- Lista todos os jogos com promoções ativas
- **Público** (não requer autenticação)

**PATCH** `/api/Game/{id}`
- Atualiza dados de um jogo
- **Requer**: Token JWT com role Admin

**DELETE** `/api/Game/{id}`
- Remove um jogo e suas promoções
- **Requer**: Token JWT com role Admin

#### 🏷️ Promoções

**POST** `/api/Promotion`
- Cria uma nova promoção para um jogo
- **Requer**: Token JWT com role Admin
- **Body**:
```json
{
  "name": "Promoção de Verão",
  "discountPercentage": 50,
  "startDate": "2024-01-01",
  "endDate": "2024-01-31",
  "gameId": 1
}
```

**GET** `/api/Promotion/{id}`
- Busca uma promoção por ID
- **Requer**: Token JWT

**PATCH** `/api/Promotion/{id}`
- Atualiza uma promoção
- **Requer**: Token JWT com role Admin

**DELETE** `/api/Promotion/{id}`
- Remove uma promoção
- **Requer**: Token JWT com role Admin

### Autenticação JWT

Para endpoints protegidos, inclua o token JWT no header:

```
Authorization: Bearer {seu-token-jwt}
```

#### Exemplo usando cURL:

```bash
# 1. Fazer login e obter o token
curl -X POST "https://localhost:5001/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@exemplo.com","password":"Admin@123"}'

# 2. Usar o token para acessar endpoint protegido
curl -X GET "https://localhost:5001/api/Game/1" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

#### Exemplo usando JavaScript (Fetch API):

```javascript
// Login
const loginResponse = await fetch('http://localhost:5001/api/Auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    email: 'usuario@exemplo.com',
    password: 'senha123'
  })
});

const { token } = await loginResponse.json();

// Usar o token
const gameResponse = await fetch('http://localhost:5001/api/Game/1', {
  headers: { 'Authorization': `Bearer ${token}` }
});

const game = await gameResponse.json();
```

## 🔒 Segurança

- **Senhas**: Criptografadas usando BCrypt
- **Autenticação**: JWT com tempo de expiração configurável
- **Autorização**: Sistema de roles (User/Admin)
- **HTTPS**: Redirecionamento automático em produção
- **Validação**: FluentValidation para validação de entrada

### Configuração JWT

As configurações de JWT estão no arquivo `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "sua-chave-secreta-aqui",
    "Issuer": "FCG.API",
    "Audience": "FCG.API"
  }
}
```

⚠️ **IMPORTANTE**: Em produção, use uma chave forte e armazene-a de forma segura (variáveis de ambiente ou Azure Key Vault).

## 🌐 Variáveis de Ambiente

Para ambientes de produção, recomenda-se usar variáveis de ambiente:

```bash
# Connection String
export ConnectionStrings__DefaultConnection="Host=seu-host;Database=FcgDataBase;Username=user;Password=pass"

# JWT Settings
export Jwt__Key="sua-chave-secreta"
export Jwt__Issuer="FCG.API"
export Jwt__Audience="FCG.API"

# ASP.NET Core Environment
export ASPNETCORE_ENVIRONMENT="Production"
```

## 🐛 Tratamento de Erros

A API utiliza um middleware customizado (`ExceptionMiddleware`) para tratamento global de exceções:

- **400**: Bad Request - Dados de entrada inválidos
- **401**: Unauthorized - Token inválido ou ausente
- **403**: Forbidden - Sem permissão para acessar o recurso
- **404**: Not Found - Recurso não encontrado
- **409**: Conflict - Conflito (ex: email já cadastrado)
- **422**: Unprocessable Entity - Entidade não pode ser processada
- **500**: Internal Server Error - Erro interno do servidor

Exemplo de resposta de erro:

```json
{
  "error": "Email already in use"
}
```

## 📝 Exemplos de Uso Completo

### Fluxo Completo: Cadastro, Login e Criação de Jogo com Promoção

```bash
# 1. Criar um usuário comum
curl -X POST "http://localhost:8080/api/User" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Maria Santos",
    "email": "maria@exemplo.com",
    "password": "Senha@123"
  }'

# 2. Fazer login
curl -X POST "http://localhost:8080/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "maria@exemplo.com",
    "password": "Senha@123"
  }'
# Resposta: { "token": "..." }

# 3. Criar um usuário admin (requer token admin)
curl -X POST "http://localhost:8080/api/User/Admin" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token-admin}" \
  -d '{
    "name": "Admin",
    "email": "admin@exemplo.com",
    "password": "Admin@123"
  }'

# 4. Cadastrar um jogo (requer token admin)
curl -X POST "http://localhost:8080/api/Game" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token-admin}" \
  -d '{
    "name": "Cyberpunk 2077",
    "description": "RPG de ação em mundo aberto futurístico",
    "price": 199.90,
    "releaseDate": "2020-12-10"
  }'
# Resposta: { "id": 1, "name": "Cyberpunk 2077", ... }

# 5. Criar uma promoção para o jogo (requer token admin)
curl -X POST "http://localhost:8080/api/Promotion" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token-admin}" \
  -d '{
    "name": "Mega Promoção",
    "discountPercentage": 50,
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "gameId": 1
  }'

# 6. Listar jogos com promoção (endpoint público)
curl -X GET "http://localhost:8080/api/Game/with-promotion"

# 7. Buscar detalhes de um jogo (requer token)
curl -X GET "http://localhost:8080/api/Game/1" \
  -H "Authorization: Bearer {seu-token}"
```

## 🤝 Contribuindo

Contribuições são sempre bem-vindas! Para contribuir:

1. Faça um Fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

### Padrões de Código

- Siga as convenções de código C#/.NET
- Escreva testes para novas funcionalidades
- Mantenha a cobertura de testes
- Documente funções públicas com XML comments
- Use FluentValidation para validações
- Siga os princípios SOLID

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 👨‍💻 Autor

**Gustavo Fontana Bouvie**

- GitHub: [@gustavofontanabouvie](https://github.com/gustavofontanabouvie)

## 📞 Suporte

Se você tiver alguma dúvida ou problema, por favor:

1. Verifique a [documentação do Swagger](http://localhost:8080/swagger)
2. Consulte os exemplos neste README
3. Abra uma [issue](https://github.com/gustavofontanabouvie/FCG.API/issues) no GitHub

## 🗺️ Roadmap

- [ ] Implementar sistema de compra de jogos
- [ ] Adicionar suporte a imagens/thumbnails de jogos
- [ ] Implementar sistema de avaliações e reviews
- [ ] Adicionar paginação nos endpoints de listagem
- [ ] Implementar cache com Redis
- [ ] Adicionar suporte a múltiplas moedas
- [ ] Criar sistema de wishlist
- [ ] Implementar notificações de promoções

## 📚 Recursos Adicionais

- [Documentação do .NET](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- [PostgreSQL](https://www.postgresql.org/docs/)
- [Docker](https://docs.docker.com/)
- [JWT.io](https://jwt.io/)

---

⭐ Se este projeto foi útil para você, considere dar uma estrela no GitHub!
