CREATE DATABASE RaceDAY;
GO

USE RaceDay;
GO

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName VARCHAR(50) NOT NULL,
    Email VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(50) NOT NULL,
    Role VARCHAR(50) NOT NULL DEFAULT 'Participant',
    CONSTRAINT CK_Users_Role CHECK (Role IN ('Organiser', 'Participant', 'Admin'))
);
GO

CREATE TABLE Events (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    Name VARCHAR(50) NOT NULL,
    Description VARCHAR(100) NOT NULL,
    EventDate DATE NOT NULL,
    Location VARCHAR(50) NOT NULL,
    RouteInfo VARCHAR(1000) NOT NULL,
    CONSTRAINT FK_Events_Organiser
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    DistanceKM DECIMAL(5,2) NOT NULL,
    Description VARCHAR(100) NOT NULL,
    CONSTRAINT UQ_Categories_Name_Distance UNIQUE (Name, DistanceKM),
    CONSTRAINT CK_Categories_Distance CHECK (DistanceKM > 0)
);
GO

CREATE TABLE Event_Categories (
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    PRIMARY KEY (EventID, CategoryID),
    CONSTRAINT FK_EventCategories_Event
    FOREIGN KEY (EventID) REFERENCES Events(EventID),
    CONSTRAINT FK_EventCategories_Category  FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);
GO

CREATE TABLE Enrolments (
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    UserID INT NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Registered'
    CONSTRAINT FK_Enrolments_Event FOREIGN KEY (EventID) REFERENCES Events(EventID),
    CONSTRAINT FK_Enrolments_Category  FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    CONSTRAINT FK_Enrolments_Users     FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT UQ_Enrolments UNIQUE (EventID, CategoryID, UserID),
    CONSTRAINT CK_Enrolments_Status  CHECK (Status IN ('Registered', 'Cancelled', 'Completed'))
);
GO

CREATE TABLE Results (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID INT NOT NULL,
    FinishTime TIME NOT NULL,
    Position INT NOT NULL,
    RecordedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Results_Enrolments
        FOREIGN KEY (EnrolmentID) REFERENCES Enrolments(EnrolmentID),

    CONSTRAINT CK_Results_Position
        CHECK (Position > 0)
);
GO



-- INSERT USERS

INSERT INTO Users (FullName, Email, Password, Role)
VALUES
('Thabo Mokoena', 'thabo@raceday.co.za', 'HASH-Sample', 'Organiser'),
('Lebo Kekae', 'leob@raceday.co.za', 'HASH-SAMPLE', 'Organiser'),
('Keletso Ngoepe', 'KN@gmail.com', 'HASH-USER121', 'Participant'),
('Katlego Nku', 'Nku@gmail.com', 'HASH-USER223', 'Participant');
GO



-- INSERT EVENTS


INSERT INTO Events
(UserID, Name, Description, EventDate, Location, RouteInfo)
VALUES
(1, 'Johannesburg City Run','Road running event','2026-10-10','Johannesburg','Flat urban route; GPX route supplied by organiser'),
(2, 'Cape Town Cycle Challenge','Cycling event','2026-11-01','Cape Town', 'Road cycling route; GPX route supplied by organiser'),(1, 'Soweto Community Walk', 'Walking event','2026-12-12','Soweto', 'Accessible community route');
GO


-- INSERT CATEGORIES


INSERT INTO Categories
(Name, DistanceKM, Description)
VALUES
('5K run', 5, '5 kilometre road run'),
('10K run', 10, '10 kilometre road run'),
('Half marathon', 21.10, '21.1 kilometre road race'),
('20K cycle', 20, '20 kilometre category'),
('50K cycle', 50, '50 kilometre cycling category'),
('5K walk', 5, '5 kilometre community walk');
GO



-- INSERT EVENT CATEGORIES


INSERT INTO Event_Categories
(EventID, CategoryID)
VALUES
(1, 1),
(1, 2),
(1, 3),
(2, 4),
(2, 5),
(3, 6);
GO


-- INSERT ENROLMENTS

INSERT INTO Enrolments
(EventID, CategoryID, UserID, Status)
VALUES
(1, 1, 3, 'Registered'),
(1, 2, 4, 'Registered'),
(2, 4, 3, 'Registered'),
(3, 6, 4, 'Registered');
GO



-- INSERT RESULTS


INSERT INTO Results
(EnrolmentID, FinishTime, Position)
VALUES
(1, '00:25:42', 12),
(2, '00:54:28', 8);
GO

