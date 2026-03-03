CREATE TABLE CandidateProfiles (
    Id INT IDENTITY PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL UNIQUE,
    FullName NVARCHAR(255) NOT NULL,
    Title NVARCHAR(255),
    Phone NVARCHAR(50),
    Location NVARCHAR(255),
    AboutMe NVARCHAR(MAX),
    AvatarUrl NVARCHAR(500),
    CVUrl NVARCHAR(500),

    FOREIGN KEY (UserId)
        REFERENCES AspNetUsers(Id)
        ON DELETE CASCADE
);

CREATE TABLE Skills (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL UNIQUE
);

CREATE TABLE CandidateSkills (
    CandidateProfileId INT NOT NULL,
    SkillId INT NOT NULL,

    PRIMARY KEY (CandidateProfileId, SkillId),

    FOREIGN KEY (CandidateProfileId)
        REFERENCES CandidateProfiles(Id)
        ON DELETE CASCADE,

    FOREIGN KEY (SkillId)
        REFERENCES Skills(Id)
        ON DELETE CASCADE
);

CREATE TABLE WorkExperiences (
    Id INT IDENTITY PRIMARY KEY,
    CandidateProfileId INT NOT NULL,
    CompanyName NVARCHAR(255) NOT NULL,
    Position NVARCHAR(255) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NULL,
    Description NVARCHAR(MAX),

    FOREIGN KEY (CandidateProfileId)
        REFERENCES CandidateProfiles(Id)
        ON DELETE CASCADE
);

CREATE TABLE Educations (
    Id INT IDENTITY PRIMARY KEY,
    CandidateProfileId INT NOT NULL,
    SchoolName NVARCHAR(255) NOT NULL,
    Degree NVARCHAR(255),
    StartDate DATE NOT NULL,
    EndDate DATE NULL,

    FOREIGN KEY (CandidateProfileId)
        REFERENCES CandidateProfiles(Id)
        ON DELETE CASCADE
);

CREATE TABLE Companies (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Location NVARCHAR(255),
    LogoUrl NVARCHAR(500),
    Description NVARCHAR(MAX),

    OwnerUserId NVARCHAR(450) NOT NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    IsVerified BIT NOT NULL DEFAULT 0,

    FOREIGN KEY (OwnerUserId)
        REFERENCES AspNetUsers(Id)
);

CREATE TABLE Categories (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Slug NVARCHAR(255) NOT NULL UNIQUE,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Jobs (
    Id INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Location NVARCHAR(255),

    SalaryMin INT NULL,
    SalaryMax INT NULL,
    IsNegotiable BIT NOT NULL DEFAULT 0,

    JobType INT NOT NULL,
    Status INT NOT NULL,

    CategoryId INT NOT NULL,
    CompanyId INT NOT NULL,
    CreatedByUserId NVARCHAR(450) NOT NULL,

    ViewsCount INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    ExpiredAt DATETIME2 NULL,

    FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id),
    FOREIGN KEY (CreatedByUserId) REFERENCES AspNetUsers(Id)
);

CREATE TABLE Applications (
    Id INT IDENTITY PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL,
    JobId INT NOT NULL,
    Status INT NOT NULL,
    AppliedAt DATETIME2 NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (UserId)
        REFERENCES AspNetUsers(Id)
        ON DELETE CASCADE,

    FOREIGN KEY (JobId)
        REFERENCES Jobs(Id)
        ON DELETE CASCADE,

    CONSTRAINT UQ_User_Job UNIQUE (UserId, JobId)
);

CREATE INDEX IX_Jobs_CompanyId ON Jobs(CompanyId);
CREATE INDEX IX_Jobs_CategoryId ON Jobs(CategoryId);
CREATE INDEX IX_Jobs_Status ON Jobs(Status);
CREATE INDEX IX_Jobs_Location ON Jobs(Location);

CREATE INDEX IX_Applications_UserId ON Applications(UserId);
CREATE INDEX IX_Applications_JobId ON Applications(JobId);