# About
OnlineBalance - open-source banking Web App for financial management

# Requirements
* .NET Core 8
* PostgreSQL

# Setup

## Clone project
Clone this project to your local and go into `./OnlineBalance` directory
```cmd
> git clone https://github.com/whyme0/OnlineBalance.git
```
## Requirements
Before running this project you should create `appsettings-Email.json` file, which needed for `EmailService` configuration. Locate newly created *json* file at the same place as `Program.cs`.

Here is example of how `appsettings-Email.json` should looks like:
```json
{
  "EmailConfiguration": {
    "From": "smtp_email_address",
    "SmtpServer": "smtp.gmail.com",
    "Port": 465,
    "Username": "smtp_username_can_be_same_email",
    "Password": "your_smtp_server_password"
  }
}
```

> P.S. You dont need to fill this field with correct data, but in such case if you will trying to interact with `EmailService` (e.g sending reset password link via email) you'll getting errors and these functions wouldn't be avialable for you

## Using docker
Run `compose.yaml` file in your terminal within directory it located
```bash
docker compose up --build -d
```
After both *(postgres and aspdotnet)* containers start working go to `localhost:8080` in your browser

To stop them all use (flag `-v` will delete volume for these containers)
```bash
docker compose down -v
```