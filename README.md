
# HTML_FC Backend (ASP.NET Core)

This is the backend API for the Football Team Management system.

## 🚀 Technologies Used

- ASP.NET Core 8.0
- SQL Server (Docker)
- Docker & Docker Compose
- JWT Authentication
- MailKit for email verification

## 🐳 Run with Docker Compose

```bash
docker-compose up -d --build
```

- API available at: http://localhost:8080/swagger
- SQL Server: `sa` / `Quypro2003@` running on port 1433

## 📁 Folder Structure

```
.
├── HTML_FC/              # ASP.NET Core project
├── LIBCORE/              # Shared business logic
├── Dockerfile            # Docker build file
├── docker-compose.yml    # Compose file for backend + SQL
└── .gitignore            # Ignore secrets and build artifacts
```

## ⚙️ Environment Configuration

Secrets like connection strings and email passwords are stored in:

```bash
appsettings.json
```

> ⚠️ This file is excluded from Git using `.gitignore`.

## 🔐 GitHub CI/CD (Planned)

This repo will integrate:

- GitHub Actions workflow to:
  - Build Docker image
  - SSH deploy to GCP VM
  - Auto-restart container
- Secrets managed via GitHub Secrets

---

Made with ❤️ by MinhQuyDev
