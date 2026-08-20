CREATE TABLE IF NOT EXISTS notificacoes_lider_pg (
  id INT NOT NULL AUTO_INCREMENT,
  titulo VARCHAR(150) NOT NULL,
  mensagem TEXT NOT NULL,
  tipo VARCHAR(50) NULL,
  link VARCHAR(300) NULL,
  data_inicio DATETIME NULL,
  data_fim DATETIME NULL,
  congregacao_id INT NULL,
  pequeno_grupo_id INT NULL,
  lider_pequeno_grupo_id INT NULL,
  status VARCHAR(1) NOT NULL DEFAULT 'A',
  data_cadastro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  INDEX ix_notificacoes_lider_pg_status (status),
  INDEX ix_notificacoes_lider_pg_congregacao_id (congregacao_id),
  INDEX ix_notificacoes_lider_pg_pequeno_grupo_id (pequeno_grupo_id),
  INDEX ix_notificacoes_lider_pg_lider_id (lider_pequeno_grupo_id),
  INDEX ix_notificacoes_lider_pg_periodo (data_inicio, data_fim)
);

CREATE TABLE IF NOT EXISTS notificacoes_lider_pg_leituras (
  id INT NOT NULL AUTO_INCREMENT,
  notificacao_id INT NOT NULL,
  lider_pequeno_grupo_id INT NOT NULL,
  data_leitura DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE KEY ux_notificacao_lider_leitura (notificacao_id, lider_pequeno_grupo_id),
  INDEX ix_notificacoes_lider_pg_leituras_lider_id (lider_pequeno_grupo_id)
);
