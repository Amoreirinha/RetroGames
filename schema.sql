-- RetroGames: criação das tabelas (MySQL 8)
CREATE DATABASE IF NOT EXISTS retrogames CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE retrogames;

CREATE TABLE IF NOT EXISTS tb_condicao_jogo (
    co_seq_condicao_jogo INT NOT NULL,
    ds_condicao_jogo     VARCHAR(40) NOT NULL,
    PRIMARY KEY (co_seq_condicao_jogo),
    UNIQUE KEY uq_condicao_jogo_ds (ds_condicao_jogo)
);

CREATE TABLE IF NOT EXISTS tb_perfil (
    co_seq_perfil INT NOT NULL,
    ds_perfil     VARCHAR(40) NOT NULL,
    PRIMARY KEY (co_seq_perfil),
    UNIQUE KEY uq_perfil_ds (ds_perfil)
);

CREATE TABLE IF NOT EXISTS tb_plataforma (
    co_seq_plataforma INT NOT NULL AUTO_INCREMENT,
    no_plataforma     VARCHAR(80) NOT NULL,
    no_fabricante     VARCHAR(80) NOT NULL,
    nu_ano_lancamento INT NOT NULL,
    PRIMARY KEY (co_seq_plataforma),
    UNIQUE KEY uq_plataforma_no (no_plataforma)
);

CREATE TABLE IF NOT EXISTS tb_genero (
    co_seq_genero INT NOT NULL AUTO_INCREMENT,
    no_genero     VARCHAR(60) NOT NULL,
    PRIMARY KEY (co_seq_genero),
    UNIQUE KEY uq_genero_no (no_genero)
);

CREATE TABLE IF NOT EXISTS tb_jogo (
    co_seq_jogo          INT NOT NULL AUTO_INCREMENT,
    no_jogo              VARCHAR(120) NOT NULL,
    ds_jogo              VARCHAR(1000) NULL,
    nu_ano_lancamento    INT NOT NULL,
    vl_preco             DECIMAL(10,2) NOT NULL,
    qt_estoque           INT NOT NULL DEFAULT 0,
    st_ativo             BOOLEAN NOT NULL DEFAULT TRUE,
    co_plataforma        INT NOT NULL,
    co_condicao_jogo     INT NOT NULL,
    PRIMARY KEY (co_seq_jogo),
    KEY ix_jogo_no (no_jogo),
    CONSTRAINT fk_jogo_plataforma FOREIGN KEY (co_plataforma)
        REFERENCES tb_plataforma (co_seq_plataforma),
    CONSTRAINT fk_jogo_condicao FOREIGN KEY (co_condicao_jogo)
        REFERENCES tb_condicao_jogo (co_seq_condicao_jogo)
);

CREATE TABLE IF NOT EXISTS tb_jogo_genero (
    co_jogo   INT NOT NULL,
    co_genero INT NOT NULL,
    PRIMARY KEY (co_jogo, co_genero),
    CONSTRAINT fk_jogo_genero_jogo FOREIGN KEY (co_jogo)
        REFERENCES tb_jogo (co_seq_jogo),
    CONSTRAINT fk_jogo_genero_genero FOREIGN KEY (co_genero)
        REFERENCES tb_genero (co_seq_genero)
);

CREATE TABLE IF NOT EXISTS tb_usuario (
    co_seq_usuario INT NOT NULL AUTO_INCREMENT,
    no_usuario     VARCHAR(100) NOT NULL,
    ds_email       VARCHAR(150) NOT NULL,
    ds_senha_hash  VARCHAR(255) NOT NULL,
    co_perfil      INT NOT NULL,
    st_ativo       BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (co_seq_usuario),
    UNIQUE KEY uq_usuario_email (ds_email),
    CONSTRAINT fk_usuario_perfil FOREIGN KEY (co_perfil)
        REFERENCES tb_perfil (co_seq_perfil)
);
