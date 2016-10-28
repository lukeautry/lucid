CREATE TABLE public.areas
(
    id integer NOT NULL DEFAULT nextval('areas_id_seq'::regclass),
    created_at timestamp with time zone,
    updated_at timestamp with time zone,
    name character varying(128) COLLATE "default".pg_catalog,
    description character varying(1024) COLLATE "default".pg_catalog,
    CONSTRAINT areas_pkey PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.areas
    OWNER to postgres;

CREATE TABLE public.rooms
(
    id integer NOT NULL DEFAULT nextval('rooms_id_seq'::regclass),
    created_at timestamp with time zone,
    updated_at timestamp with time zone,
    name character varying(128) COLLATE "default".pg_catalog,
    description character varying(1024) COLLATE "default".pg_catalog,
    area_id integer,
    north_room_id integer,
    east_room_id integer,
    south_room_id integer,
    west_room_id integer,
    up_room_id integer,
    down_room_id integer,
    CONSTRAINT rooms_pkey PRIMARY KEY (id),
    CONSTRAINT rooms_area_id_foreign FOREIGN KEY (area_id)
        REFERENCES public.areas (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT rooms_down_room_id_foreign FOREIGN KEY (down_room_id)
        REFERENCES public.rooms (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE SET NULL,
    CONSTRAINT rooms_east_room_id_foreign FOREIGN KEY (east_room_id)
        REFERENCES public.rooms (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE SET NULL,
    CONSTRAINT rooms_north_room_id_foreign FOREIGN KEY (north_room_id)
        REFERENCES public.rooms (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE SET NULL,
    CONSTRAINT rooms_south_room_id_foreign FOREIGN KEY (south_room_id)
        REFERENCES public.rooms (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE SET NULL,
    CONSTRAINT rooms_up_room_id_foreign FOREIGN KEY (up_room_id)
        REFERENCES public.rooms (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE SET NULL,
    CONSTRAINT rooms_west_room_id_foreign FOREIGN KEY (west_room_id)
        REFERENCES public.rooms (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE SET NULL
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.rooms
    OWNER to postgres;

CREATE TABLE public.users
(
    id integer NOT NULL DEFAULT nextval('users_id_seq'::regclass),
    created_at timestamp with time zone,
    updated_at timestamp with time zone,
    name character varying(64) COLLATE "default".pg_catalog,
    hashed_password character varying(255) COLLATE "default".pg_catalog,
    CONSTRAINT users_pkey PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.users
    OWNER to postgres;