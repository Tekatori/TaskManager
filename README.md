SQL
USE TaskManagerDB;
-- 1. Users
CREATE TABLE Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    Role VARCHAR(50) NOT NULL DEFAULT 'User',
    CreatedAt DATETIME
);

-- 2. Projects
CREATE TABLE Projects (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    OwnerId INT,  -- Giữ lại OwnerId để liên kết với Users(Id)
    CreatedAt DATETIME
);

-- 3. Tasks
CREATE TABLE Tasks (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Description TEXT,
    Status ENUM('Pending', 'InProgress', 'Done') DEFAULT 'Pending',
    Priority ENUM('Low', 'Medium', 'High') DEFAULT 'Medium',
    DueDate DATETIME,
    ProjectId INT,      -- Giữ để liên kết Projects
    AssignedTo INT,     -- Giữ để liên kết Users
    CreatedAt DATETIME,
    UpdatedAt DATETIME
);

-- 4. Comments
CREATE TABLE Comments (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    TaskId INT NOT NULL,   -- Liên kết với Tasks
    UserId INT NOT NULL,   -- Liên kết với Users
    Content TEXT NOT NULL,
    CreatedAt DATETIME
);

-- 5. Attachments
CREATE TABLE Attachments (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    TaskId INT NOT NULL,       -- Liên kết với Tasks
    FileName VARCHAR(255) NOT NULL,
    FileUrl TEXT NOT NULL,
    UploadedAt DATETIME
);
