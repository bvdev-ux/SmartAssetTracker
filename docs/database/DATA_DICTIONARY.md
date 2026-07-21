# Diccionario de Datos

## users

| Columna | Tipo | Restricciones | Descripción |
|---------|------|---------------|-------------|
| id | UUID | PK | Identificador único |
| email | VARCHAR(256) | UNIQUE, NOT NULL | Correo de acceso |
| password_hash | VARCHAR(512) | NOT NULL | Hash PBKDF2-SHA256 |
| full_name | VARCHAR(200) | NOT NULL | Nombre completo |
| role_id | UUID | FK → roles | Rol asignado |
| is_active | BOOLEAN | DEFAULT true | Estado del usuario |
| last_login_at | TIMESTAMPTZ | NULL | Último inicio de sesión |
| created_at | TIMESTAMPTZ | NOT NULL | Fecha de creación |
| updated_at | TIMESTAMPTZ | NULL | Última actualización |

## assets

| Columna | Tipo | Restricciones | Descripción |
|---------|------|---------------|-------------|
| id | UUID | PK | Identificador único |
| asset_tag | VARCHAR(50) | UNIQUE, NOT NULL | Etiqueta institucional |
| serial_number | VARCHAR(100) | NULL | Número de serie del fabricante |
| qr_code | VARCHAR(256) | NULL | Código QR asociado |
| rfid_tag | VARCHAR(256) | NULL | Reservado para RFID futuro |
| category_id | UUID | FK → asset_categories | Categoría del equipo |
| model_id | UUID | FK → asset_models | Modelo del equipo |
| status | SMALLINT | NOT NULL | Estado (enum AssetStatusType) |
| current_location_id | UUID | FK → locations, NULL | Ubicación registrada |
| is_inside_campus | BOOLEAN | DEFAULT true | Si está dentro del campus |
| notes | TEXT | NULL | Observaciones |

## asset_movements

| Columna | Tipo | Restricciones | Descripción |
|---------|------|---------------|-------------|
| id | UUID | PK | Identificador único |
| asset_id | UUID | FK → assets | Activo involucrado |
| person_id | UUID | FK → people | Persona responsable |
| location_id | UUID | FK → locations | Punto de registro |
| movement_type | SMALLINT | NOT NULL | Entry, Exit, ReEntry |
| occurred_at | TIMESTAMPTZ | NOT NULL, INDEX | Momento del movimiento |
| validation_method | VARCHAR(50) | NULL | QR, manual, RFID (futuro) |
| notes | TEXT | NULL | Observaciones |

## alerts

| Columna | Tipo | Restricciones | Descripción |
|---------|------|---------------|-------------|
| id | UUID | PK | Identificador único |
| asset_id | UUID | FK → assets, NULL | Activo relacionado |
| alert_type | SMALLINT | NOT NULL | Tipo de alerta |
| severity | SMALLINT | NOT NULL | Low, Medium, High, Critical |
| status | SMALLINT | NOT NULL | Active, Acknowledged, Resolved |
| message | VARCHAR(500) | NOT NULL | Descripción de la alerta |
| resolved_at | TIMESTAMPTZ | NULL | Fecha de resolución |
| resolved_by | UUID | NULL | Usuario que resolvió |

## audit_logs

| Columna | Tipo | Restricciones | Descripción |
|---------|------|---------------|-------------|
| id | UUID | PK | Identificador único |
| user_id | UUID | FK → users, NULL | Usuario que ejecutó la acción |
| action | SMALLINT | NOT NULL | Create, Update, Delete, Login, etc. |
| entity_name | VARCHAR(100) | NOT NULL | Nombre de la entidad afectada |
| entity_id | UUID | NULL | ID del registro afectado |
| details | TEXT | NULL | Detalle adicional en JSON/texto |
| ip_address | VARCHAR(45) | NULL | Dirección IP del cliente |
| occurred_at | TIMESTAMPTZ | NOT NULL, INDEX | Momento del evento |
