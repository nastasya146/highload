create table if not exists public.users (
    user_id uuid primary key,
    password varchar(255) not null,
    first_name varchar(255) not null,
    last_name varchar(255) not null,
    birth_date date not null,
    gender varchar(50) not null,
    city varchar(255) not null,
    interests text not null
);