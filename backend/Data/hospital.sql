--
-- PostgreSQL database dump
--

\restrict rjQw97KdGEPnmLW633Ka0PekScm2cWIj2KCX1jlCDaXxvKflEncjFChztg2SgGk

-- Dumped from database version 16.14 (Ubuntu 16.14-0ubuntu0.24.04.1)
-- Dumped by pg_dump version 16.14 (Ubuntu 16.14-0ubuntu0.24.04.1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: chambre; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.chambre (
    numchambre integer NOT NULL,
    nbr_lit integer NOT NULL
);


ALTER TABLE public.chambre OWNER TO postgres;

--
-- Name: chambre_numchambre_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.chambre_numchambre_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.chambre_numchambre_seq OWNER TO postgres;

--
-- Name: chambre_numchambre_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.chambre_numchambre_seq OWNED BY public.chambre.numchambre;


--
-- Name: consultation; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.consultation (
    refcons integer NOT NULL,
    type character varying(50) NOT NULL,
    date timestamp without time zone NOT NULL,
    frais integer DEFAULT 0,
    idmed integer,
    idpatient integer NOT NULL,
    prescription text,
    objetfacture text,
    etatfacture character varying(25)
);


ALTER TABLE public.consultation OWNER TO postgres;

--
-- Name: consultation_refcons_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.consultation_refcons_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.consultation_refcons_seq OWNER TO postgres;

--
-- Name: consultation_refcons_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.consultation_refcons_seq OWNED BY public.consultation.refcons;


--
-- Name: medecin; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.medecin (
    idmed integer NOT NULL,
    nommed character varying(255) NOT NULL,
    poste character varying(50) NOT NULL,
    codemed character varying(255) NOT NULL
);


ALTER TABLE public.medecin OWNER TO postgres;

--
-- Name: medecin_idmed_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.medecin_idmed_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.medecin_idmed_seq OWNER TO postgres;

--
-- Name: medecin_idmed_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.medecin_idmed_seq OWNED BY public.medecin.idmed;


--
-- Name: patient; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.patient (
    idpatient integer NOT NULL,
    nompatient character varying(255) NOT NULL,
    etat character varying(50) NOT NULL,
    numchambre integer,
    datehosp date,
    datesortie date,
    numpatient character varying(15)
);


ALTER TABLE public.patient OWNER TO postgres;

--
-- Name: patient_idpatient_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.patient_idpatient_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.patient_idpatient_seq OWNER TO postgres;

--
-- Name: patient_idpatient_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.patient_idpatient_seq OWNED BY public.patient.idpatient;


--
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    iduser integer NOT NULL,
    login character varying(255) NOT NULL,
    type character varying(25) NOT NULL,
    password character varying(255) NOT NULL
);


ALTER TABLE public.users OWNER TO postgres;

--
-- Name: users_iduser_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.users_iduser_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.users_iduser_seq OWNER TO postgres;

--
-- Name: users_iduser_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_iduser_seq OWNED BY public.users.iduser;


--
-- Name: chambre numchambre; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.chambre ALTER COLUMN numchambre SET DEFAULT nextval('public.chambre_numchambre_seq'::regclass);


--
-- Name: consultation refcons; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.consultation ALTER COLUMN refcons SET DEFAULT nextval('public.consultation_refcons_seq'::regclass);


--
-- Name: medecin idmed; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.medecin ALTER COLUMN idmed SET DEFAULT nextval('public.medecin_idmed_seq'::regclass);


--
-- Name: patient idpatient; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.patient ALTER COLUMN idpatient SET DEFAULT nextval('public.patient_idpatient_seq'::regclass);


--
-- Name: users iduser; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN iduser SET DEFAULT nextval('public.users_iduser_seq'::regclass);


--
-- Data for Name: chambre; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.chambre (numchambre, nbr_lit) FROM stdin;
1	4
\.


--
-- Data for Name: consultation; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.consultation (refcons, type, date, frais, idmed, idpatient, prescription, objetfacture, etatfacture) FROM stdin;
1	Generale	2026-08-16 15:45:00	0	9	4	\N	\N	\N
2	Générale	2026-08-17 16:30:00	0	4	5	\N	\N	Non-Payé
\.


--
-- Data for Name: medecin; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.medecin (idmed, nommed, poste, codemed) FROM stdin;
4	Lucas	Généraliste	MED002
3	Luc Anton	Dentiste	MED001
9	Bertrand	Généraliste	MED003
10	Bernard	Généraliste	MED004
\.


--
-- Data for Name: patient; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.patient (idpatient, nompatient, etat, numchambre, datehosp, datesortie, numpatient) FROM stdin;
4	dupont	Non-Hospitalisé	\N	\N	\N	PAT001
5	Dexter	Non-Hospitalisé	\N	\N	\N	PAT002
6	Benjamin	Non-Hospitalisé	\N	\N	\N	PAT003
\.


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (iduser, login, type, password) FROM stdin;
1	Timoty	testeur	b2698f72af84bd18d47193fd56d131aa102be7537610dbee9234c3ed2690ab62
2	Acceuil	Acceuil	ee465a80171432cb5152a3e7a898a5f898c1a1dbd1fb5a47c3ea15383069e89b
5	MED001	medecin	cd3b31c8ace043f690effabe2d7d01b2bfa17140df83bef02dcc31a93c83acb4
6	MED002	medecin	52d7d8604bf71e968bf07d468c7d394bff7cb0bb142fd4da7a85dd6f8056a940
11	MED003	medecin	eeb2701607786e9d2efd96d21eb69c1d3eaee67eb466463932fe4f652d75ae20
12	MED004	medecin	3a4d5b86cfa208b50f1175f39165355a3b05a4ce8ef30fa6ee9ea326905efe9b
\.


--
-- Name: chambre_numchambre_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.chambre_numchambre_seq', 1, true);


--
-- Name: consultation_refcons_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.consultation_refcons_seq', 3, true);


--
-- Name: medecin_idmed_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.medecin_idmed_seq', 10, true);


--
-- Name: patient_idpatient_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.patient_idpatient_seq', 6, true);


--
-- Name: users_iduser_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.users_iduser_seq', 12, true);


--
-- Name: chambre chambre_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.chambre
    ADD CONSTRAINT chambre_pkey PRIMARY KEY (numchambre);


--
-- Name: consultation consultation_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.consultation
    ADD CONSTRAINT consultation_pkey PRIMARY KEY (refcons);


--
-- Name: medecin medecin_codemed_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.medecin
    ADD CONSTRAINT medecin_codemed_key UNIQUE (codemed);


--
-- Name: medecin medecin_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.medecin
    ADD CONSTRAINT medecin_pkey PRIMARY KEY (idmed);


--
-- Name: patient patient_numpatien_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.patient
    ADD CONSTRAINT patient_numpatien_key UNIQUE (numpatient);


--
-- Name: patient patient_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.patient
    ADD CONSTRAINT patient_pkey PRIMARY KEY (idpatient);


--
-- Name: users users_login_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_login_key UNIQUE (login);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (iduser);


--
-- Name: consultation consultation_idmed_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.consultation
    ADD CONSTRAINT consultation_idmed_fkey FOREIGN KEY (idmed) REFERENCES public.medecin(idmed) ON DELETE CASCADE;


--
-- Name: consultation consultation_numpatient_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.consultation
    ADD CONSTRAINT consultation_numpatient_fkey FOREIGN KEY (idpatient) REFERENCES public.patient(idpatient) ON DELETE CASCADE;


--
-- Name: patient patient_numchambre_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.patient
    ADD CONSTRAINT patient_numchambre_fkey FOREIGN KEY (numchambre) REFERENCES public.chambre(numchambre) ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict rjQw97KdGEPnmLW633Ka0PekScm2cWIj2KCX1jlCDaXxvKflEncjFChztg2SgGk

