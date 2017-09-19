CREATE TABLE players (
	id serial,
	name text NOT NULl,
	hash text NOT NULL,
	token text,
	health integer NOT NULL DEFAULT 100,
	position integer NOT NULL DEFAULT 0
);

CREATE TABLE items (
	item_id integer NOT NULL,
	player_id integer NOT NULL,
	position integer NOT NULL,
	count integer NOT NULL,
	PRIMARY KEY (player_id, position)
);