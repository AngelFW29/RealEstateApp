# RealEstateApp

## Integrantes

1. Welinton De Jesus Corporan Fabal - Matrícula: `2025-1073`
2. Yerlin Rojas Gálvez - Matrícula: `2021-0540`
3. Angel Farrell Mosquea Rosa - Matrícula: `2025-1091` 

## Seeds

El proyecto incluye usuarios iniciales para facilitar las pruebas del sistema.

| Usuario | Contraseña | Rol |
|---|---|---|
| admin | `Pa$$word123` | Administrador |
| developer | `Pa$$word123` | Developer |
| agent | `Pa$$word123` | Agente |
| client | `Pa$$word123` | Cliente |

## Mailtrap

Se utiliza [Mailtrap](https://mailtrap.io/) para realizar pruebas de envío de correos electrónicos mediante SMTP.

### Configuración

1. Crear una cuenta en [Mailtrap](https://mailtrap.io/).
2. Seleccionar un **Email Testing Inbox**.
3. Consultar las credenciales SMTP proporcionadas por Mailtrap.
4. Puede ver como la configuramos en appsettings.Development.

Las claves de configuración utilizadas incluyen:

- `MailSettings:SmtpUser`
- `MailSettings:SmtpPass`

El host y el puerto SMTP deben configurarse según los datos proporcionados por Mailtrap.