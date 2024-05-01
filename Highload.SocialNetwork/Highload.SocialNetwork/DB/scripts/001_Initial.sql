create table if not exists public.users (
    user_id uuid DEFAULT gen_random_uuid () primary key,
    password varchar(255) not null,
    first_name varchar(255) not null,
    last_name varchar(255) not null,
    birth_date date not null,
    gender varchar(50) null,
    city varchar(255) not null,
    interests text null
);

copy users (last_name, first_name, birth_date, city, password) FROM '/tmp/people.v2.2.csv' CSV HEADER;