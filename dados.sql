-- RetroGames: dados de exemplo (rodar depois do schema.sql). Não insere usuários.
USE retrogames;

INSERT INTO tb_condicao_jogo (co_seq_condicao_jogo, ds_condicao_jogo) VALUES
 (1, 'Novo'), (2, 'Usado'), (3, 'Recondicionado');

INSERT INTO tb_perfil (co_seq_perfil, ds_perfil) VALUES
 (1, 'Administrador'), (2, 'Cliente');

INSERT INTO tb_plataforma (co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento) VALUES
 (1, 'Super Nintendo', 'Nintendo', 1990),
 (2, 'Mega Drive', 'Sega', 1988),
 (3, 'PlayStation', 'Sony', 1994),
 (4, 'Nintendo 64', 'Nintendo', 1996),
 (5, 'Atari 2600', 'Atari', 1977),
 (6, 'Game Boy', 'Nintendo', 1989);

INSERT INTO tb_genero (co_seq_genero, no_genero) VALUES
 (1, 'Ação'), (2, 'Aventura'), (3, 'Plataforma'), (4, 'RPG'),
 (5, 'Luta'), (6, 'Corrida'), (7, 'Esporte'), (8, 'Quebra-cabeça');

-- (id, nome, descrição, ano, preço, estoque, ativo, plataforma, condição)
INSERT INTO tb_jogo (co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, co_plataforma, co_condicao_jogo) VALUES
 (1,  'Super Mario World', 'Mario e Yoshi exploram a Dinosaur Land.', 1990, 189.90, 5, TRUE, 1, 2),
 (2,  'The Legend of Zelda: A Link to the Past', 'Link salva Hyrule em dois mundos.', 1991, 249.90, 3, TRUE, 1, 2),
 (3,  'Super Metroid', 'Samus enfrenta Mother Brain em Zebes.', 1994, 229.90, 2, TRUE, 1, 3),
 (4,  'Street Fighter II', 'Clássico de luta com oito lutadores.', 1991, 159.90, 4, TRUE, 1, 2),
 (5,  'Chrono Trigger', 'RPG sobre viagem no tempo.', 1995, 399.90, 0, TRUE, 1, 2),
 (6,  'Sonic the Hedgehog', 'O ouriço mais rápido contra o Dr. Robotnik.', 1991, 119.90, 6, TRUE, 2, 2),
 (7,  'Streets of Rage 2', 'Beat em up cooperativo nas ruas.', 1992, 179.90, 3, TRUE, 2, 2),
 (8,  'Phantasy Star IV', 'RPG de ficção científica da Sega.', 1993, 299.90, 1, TRUE, 2, 3),
 (9,  'Mortal Kombat II', 'Luta com golpes finais sangrentos.', 1993, 139.90, 4, TRUE, 2, 2),
 (10, 'Final Fantasy VII', 'Cloud e a luta contra a Shinra.', 1997, 189.90, 3, TRUE, 3, 2),
 (11, 'Crash Bandicoot', 'Aventura de plataforma em ilhas tropicais.', 1996, 129.90, 5, TRUE, 3, 2),
 (12, 'Gran Turismo', 'Simulador de corrida realista.', 1997, 99.90, 7, TRUE, 3, 3),
 (13, 'Tekken 3', 'Luta 3D com grande elenco.', 1997, 119.90, 4, TRUE, 3, 2),
 (14, 'Super Mario 64', 'Mario em 3D no castelo da Peach.', 1996, 279.90, 2, TRUE, 4, 2),
 (15, 'GoldenEye 007', 'Tiro em primeira pessoa do agente 007.', 1997, 219.90, 3, TRUE, 4, 2),
 (16, 'Mario Kart 64', 'Corrida de karts com itens.', 1996, 199.90, 4, TRUE, 4, 3),
 (17, 'Pitfall!', 'Explorador na selva, pioneiro do gênero.', 1982, 79.90, 8, TRUE, 5, 2),
 (18, 'Space Invaders', 'Defenda a Terra de invasores.', 1980, 69.90, 5, FALSE, 5, 2),
 (19, 'Tetris', 'O quebra-cabeça de blocos mais famoso.', 1989, 89.90, 10, TRUE, 6, 1),
 (20, 'Pokémon Red', 'Capture 151 monstros de bolso.', 1996, 249.90, 3, TRUE, 6, 2);

INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
 (1,3), (2,2), (2,1), (3,1), (3,2), (4,5), (5,4), (5,2),
 (6,3), (7,1), (8,4), (9,5),
 (10,4), (10,2), (11,3), (11,1), (12,6), (12,7), (13,5),
 (14,3), (14,2), (15,1), (16,6),
 (17,3), (17,2), (18,1), (19,8), (20,4), (20,2);
