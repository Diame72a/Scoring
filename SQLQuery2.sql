USE MonProjetL3_DB;
GO

PRINT '--- DÉBUT DU SCRIPT DE PEUPLEMENT MASSIF ---';

-- ============================================================
-- 1. NETTOYAGE (Reset total pour éviter les doublons)
-- ============================================================
DELETE FROM ParametreScoring;
DELETE FROM CompetenceAcquise;
DELETE FROM CompetenceSouhaitee;
DELETE FROM Offre;
DELETE FROM Personne;
DELETE FROM Poste;
DELETE FROM Competence;

-- Remise à zéro des compteurs d'ID (Auto-Increment)
DBCC CHECKIDENT ('Poste', RESEED, 0);
DBCC CHECKIDENT ('Competence', RESEED, 0);
DBCC CHECKIDENT ('Offre', RESEED, 0);
DBCC CHECKIDENT ('Personne', RESEED, 0);

PRINT 'Nettoyage terminé.';

-- ============================================================
-- 2. CRÉATION DU RÉFÉRENTIEL (Compétences & Métiers)
-- ============================================================

-- A. Compétences
INSERT INTO Competence (Nom) VALUES 
('C#'), ('ASP.NET Core'), ('Entity Framework'), ('SQL Server'), -- Backend Microsoft
('Java'), ('Spring Boot'), -- Backend Java
('React'), ('Angular'), ('Vue.js'), ('HTML/CSS'), -- Frontend
('Python'), ('Machine Learning'), ('Data Analysis'), -- Data
('Docker'), ('Kubernetes'), -- DevOps
('Anglais'), ('Gestion de Projet'), ('Agile/Scrum'); -- Soft Skills

-- B. Postes
INSERT INTO Poste (Intitule) VALUES 
('Développeur Fullstack .NET'), 
('Développeur Front-End'),
('Data Scientist'),
('Chef de Projet IT'),
('Ingénieur DevOps');

PRINT 'Référentiels créés.';

-- ============================================================
-- 3. CRÉATION DES OFFRES (La Demande)
-- ============================================================

-- VARIABLES TEMPORAIRES POUR LES ID
DECLARE @OffreId INT;
DECLARE @PosteId INT;

-- --- OFFRE 1 : DEV .NET SENIOR (LILLE) ---
SELECT @PosteId = Id FROM Poste WHERE Intitule = 'Développeur Fullstack .NET';
INSERT INTO Offre (Titre, Description, DateCreation, VilleCible, CodePostalCible, PosteId)
VALUES ('Tech Lead .NET (H/F)', 'Leader technique pour refonte architecture micro-services. Expertise C# exigée.', GETDATE(), 'Lille', '59000', @PosteId);
SET @OffreId = SCOPE_IDENTITY();

-- Poids : Compétence Roi (70%), Expérience importante (20%), Ville flexible (10%)
INSERT INTO ParametreScoring (OffreId, PoidsCompetences, PoidsExperience, PoidsLocalisation, ExclureSiVilleDiff)
VALUES (@OffreId, 70, 20, 10, 0);

-- Compétences requises
INSERT INTO CompetenceSouhaitee (OffreId, CompetenceId, NiveauRequis) VALUES 
(@OffreId, (SELECT Id FROM Competence WHERE Nom = 'C#'), 5),
(@OffreId, (SELECT Id FROM Competence WHERE Nom = 'ASP.NET Core'), 5),
(@OffreId, (SELECT Id FROM Competence WHERE Nom = 'SQL Server'), 4),
(@OffreId, (SELECT Id FROM Competence WHERE Nom = 'Anglais'), 3);


-- --- OFFRE 2 : DATA SCIENTIST (PARIS) ---
SELECT @PosteId = Id FROM Poste WHERE Intitule = 'Data Scientist';
INSERT INTO Offre (Titre, Description, DateCreation, VilleCible, CodePostalCible, PosteId)
VALUES ('Data Scientist Junior', 'Analyse prédictive et ML. Profil mathématicien bienvenu.', GETDATE(), 'Paris', '75001', @PosteId);
SET @OffreId = SCOPE_IDENTITY();

-- Poids : Équilibré, mais exclusion si pas la bonne ville !
INSERT INTO ParametreScoring (OffreId, PoidsCompetences, PoidsExperience, PoidsLocalisation, ExclureSiVilleDiff)
VALUES (@OffreId, 50, 10, 40, 1); -- EXCLUSION ACTIVE

-- Compétences requises
INSERT INTO CompetenceSouhaitee (OffreId, CompetenceId, NiveauRequis) VALUES 
(@OffreId, (SELECT Id FROM Competence WHERE Nom = 'Python'), 4),
(@OffreId, (SELECT Id FROM Competence WHERE Nom = 'Machine Learning'), 3),
(@OffreId, (SELECT Id FROM Competence WHERE Nom = 'Data Analysis'), 4);


-- --- OFFRE 3 : STAGE FRONT-END (LYON) ---
SELECT @PosteId = Id FROM Poste WHERE Intitule = 'Développeur Front-End';
INSERT INTO Offre (Titre, Description, DateCreation, VilleCible, CodePostalCible, PosteId)
VALUES ('Stage React.js (6 mois)', 'Rejoignez une startup dynamique. Possibilité d''embauche.', GETDATE(), 'Lyon', '69000', @PosteId);
SET @OffreId = SCOPE_IDENTITY();

-- Poids : La ville compte énormément (60%) car c'est un stage
INSERT INTO ParametreScoring (OffreId, PoidsCompetences, PoidsExperience, PoidsLocalisation, ExclureSiVilleDiff)
VALUES (@OffreId, 30, 10, 60, 0);

-- Compétences requises
INSERT INTO CompetenceSouhaitee (OffreId, CompetenceId, NiveauRequis) VALUES 
(@OffreId, (SELECT Id FROM Competence WHERE Nom = 'React'), 3),
(@OffreId, (SELECT Id FROM Competence WHERE Nom = 'HTML/CSS'), 3);


-- --- OFFRE 4 : CHEF DE PROJET (LILLE) ---
SELECT @PosteId = Id FROM Poste WHERE Intitule = 'Chef de Projet IT';
INSERT INTO Offre (Titre, Description, DateCreation, VilleCible, CodePostalCible, PosteId)
VALUES ('Chef de Projet Agile', 'Pilotage de projets web complexes.', DATEADD(day, -5, GETDATE()), 'Lille', '59000', @PosteId);
SET @OffreId = SCOPE_IDENTITY();

INSERT INTO ParametreScoring (OffreId, PoidsCompetences, PoidsExperience, PoidsLocalisation, ExclureSiVilleDiff)
VALUES (@OffreId, 40, 40, 20, 0);

INSERT INTO CompetenceSouhaitee (OffreId, CompetenceId, NiveauRequis) VALUES 
(@OffreId, (SELECT Id FROM Competence WHERE Nom = 'Gestion de Projet'), 5),
(@OffreId, (SELECT Id FROM Competence WHERE Nom = 'Agile/Scrum'), 4),
(@OffreId, (SELECT Id FROM Competence WHERE Nom = 'Anglais'), 4);

PRINT 'Offres créées.';

-- ============================================================
-- 4. CRÉATION DES CANDIDATS (L'Offre)
-- ============================================================

DECLARE @PersonneId INT;

-- CANDIDAT 1 : THOMAS (Le Senior Parfait pour Lille)
INSERT INTO Personne (Nom, Prenom, Email, DateNaissance, Ville, CodePostal, AnneesExperienceTotal)
VALUES ('Dubois', 'Thomas', 'thomas.dubois@email.com', '1990-05-12', 'Lille', '59000', 8);
SET @PersonneId = SCOPE_IDENTITY();

INSERT INTO CompetenceAcquise (PersonneId, CompetenceId, Niveau) VALUES 
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'C#'), 5),
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'ASP.NET Core'), 5),
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'SQL Server'), 5),
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'Anglais'), 4),
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'Docker'), 3);


-- CANDIDAT 2 : SARAH (La Junior Prometteuse pour Lyon/Front)
INSERT INTO Personne (Nom, Prenom, Email, DateNaissance, Ville, CodePostal, AnneesExperienceTotal)
VALUES ('Martin', 'Sarah', 'sarah.martin@email.com', '2002-08-24', 'Lyon', '69002', 1);
SET @PersonneId = SCOPE_IDENTITY();

INSERT INTO CompetenceAcquise (PersonneId, CompetenceId, Niveau) VALUES 
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'React'), 4),
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'HTML/CSS'), 5),
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'Anglais'), 3);


-- CANDIDAT 3 : KARIM (L'Expert Data Parisien)
INSERT INTO Personne (Nom, Prenom, Email, DateNaissance, Ville, CodePostal, AnneesExperienceTotal)
VALUES ('Benzema', 'Karim', 'kbenzema@email.com', '1995-12-19', 'Paris', '75010', 5);
SET @PersonneId = SCOPE_IDENTITY();

INSERT INTO CompetenceAcquise (PersonneId, CompetenceId, Niveau) VALUES 
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'Python'), 5),
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'Machine Learning'), 4),
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'Data Analysis'), 5),
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'SQL Server'), 3);


-- CANDIDAT 4 : JULIE (Le profil "Entre deux" pour Lille)
-- Elle postule pour le Tech Lead mais elle n'a que 3 ans d'XP et niveau moyen.
INSERT INTO Personne (Nom, Prenom, Email, DateNaissance, Ville, CodePostal, AnneesExperienceTotal)
VALUES ('Lefebvre', 'Julie', 'julie.lfb@email.com', '1999-03-10', 'Lille', '59000', 3);
SET @PersonneId = SCOPE_IDENTITY();

INSERT INTO CompetenceAcquise (PersonneId, CompetenceId, Niveau) VALUES 
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'C#'), 3),
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'ASP.NET Core'), 3),
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'React'), 3);


-- CANDIDAT 5 : MARC (L'erreur de casting)
-- Il est Chef de Projet mais habite à Marseille (alors qu'on cherche à Lille).
INSERT INTO Personne (Nom, Prenom, Email, DateNaissance, Ville, CodePostal, AnneesExperienceTotal)
VALUES ('Deschamps', 'Marc', 'mdeschamps@email.com', '1985-10-10', 'Marseille', '13000', 15);
SET @PersonneId = SCOPE_IDENTITY();

INSERT INTO CompetenceAcquise (PersonneId, CompetenceId, Niveau) VALUES 
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'Gestion de Projet'), 5),
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'Agile/Scrum'), 5);


-- CANDIDAT 6 : SOPHIE (La Polyvalente)
INSERT INTO Personne (Nom, Prenom, Email, DateNaissance, Ville, CodePostal, AnneesExperienceTotal)
VALUES ('Marceau', 'Sophie', 'soso@email.com', '1992-01-01', 'Lille', '59800', 6);
SET @PersonneId = SCOPE_IDENTITY();

INSERT INTO CompetenceAcquise (PersonneId, CompetenceId, Niveau) VALUES 
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'Java'), 4),
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'Spring Boot'), 4),
(@PersonneId, (SELECT Id FROM Competence WHERE Nom = 'Anglais'), 5); -- Elle est bonne mais pas en .NET !

PRINT 'Candidats créés.';
PRINT '--- FIN DU SCRIPT : DONNÉES PRÊTES ---';
GO