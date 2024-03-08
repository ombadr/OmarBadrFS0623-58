CREATE TABLE Clienti (
	CodiceFiscale VARCHAR(16) PRIMARY KEY,
	Cognome VARCHAR(50),
	Nome VARCHAR(50),
	Citta VARCHAR(50),
	Provincia VARCHAR(50),
	Email VARCHAR(100),
	Telefono VARCHAR(20),
	Cellulare VARCHAR(20), 
);

CREATE TABLE Camere (
	Numero INT PRIMARY KEY,
	Descrizione NVARCHAR(100),
	Tipologia VARCHAR(50)
);

CREATE TABLE Prenotazioni (
	Id INT PRIMARY KEY IDENTITY(1,1),
	CodiceFiscaleCliente VARCHAR(16),
	NumeroCamera INT,
	DataPrenotazione DATETIME,
	NumeroProgressivo INT,
	Anno INT,
	DataInizio DATETIME,
	DataFine DATETIME,
	CaparraConfermatoria DECIMAL(10, 2),
	TariffaApplicata DECIMAL(10, 2),
	DettagliSoggiorno VARCHAR(255),
	FOREIGN KEY (CodiceFiscaleCliente) REFERENCES Clienti(CodiceFiscale),
	FOREIGN KEY (NumeroCamera) REFERENCES Camere(Numero)
);

CREATE TABLE ServiziAggiuntivi (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PrenotazioneId INT,
	ServizioId INT,
	Data DATETIME,
	Quantita INT,
	FOREIGN KEY (PrenotazioneId) REFERENCES Prenotazioni(Id),
	FOREIGN KEY (ServizioId) REFERENCES ListaServizi(Id)
);

CREATE TABLE Ruoli (
	RuoloId INT IDENTITY(1,1) PRIMARY KEY,
	Nome NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Utenti(
	UtenteId INT IDENTITY(1,1) PRIMARY KEY,
	Username NVARCHAR(50) NOT NULL UNIQUE,
	Password NVARCHAR(255) NOT NULL,
	Email NVARCHAR(255) NOT NULL UNIQUE,
	RuoloId INT,
	CONSTRAINT FK_Utenti_Ruoli FOREIGN KEY (RuoloId) REFERENCES Ruoli(RuoloId)
);

INSERT INTO Ruoli (Nome) VALUES ('Admin');
INSERT INTO Ruoli (Nome) VALUES ('Operator');

CREATE TABLE ListaServizi (
	Id INT PRIMARY KEY IDENTITY(1,1),
	Descrizione VARCHAR(255),
	Prezzo DECIMAL (10, 2)
);

INSERT INTO ListaServizi (Descrizione, Prezzo) VALUES
('Colazione in camera', 20.00),
('Bevande e cibo nel mini bar', 15.00),
('Internet', 5.00),
('Letto aggiuntivo', 30.00),
('Culla', 10.00);

ALTER TABLE ServiziAggiuntivi
ADD PrezzoTotale DECIMAL(10, 2);
