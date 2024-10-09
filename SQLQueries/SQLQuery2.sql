INSERT INTO AspNetUsers (
    Id,
    UserName,
    NormalizedUserName,
    Email,
    NormalizedEmail,
    EmailConfirmed,
    PasswordHash,
    SecurityStamp,
    ConcurrencyStamp,
    PhoneNumber,
    PhoneNumberConfirmed,
    TwoFactorEnabled,
    LockoutEnd,
    LockoutEnabled,
    AccessFailedCount
) 
VALUES (
    'c39c78c3-3d5b-44ab-90ec-a5a7f5ddff9b', -- Sample GUID
    'testuser',
    'TESTUSER',
    'testuser@example.com',
    'TESTUSER@EXAMPLE.COM',
    1,
    'AQAAAAIAAYagAAAAEBBmDvM8SZZkrAh+dJHVRDF2oEwOF4b7EeZzJds5Tt8SeWFlTQ==', -- Sample password hash
    '6d62b972-b8d9-4e68-bbfa-9c43f3eec3f2',
    '1294aa12-7c3c-48b6-9b0e-f77eddfb2917',
    NULL,
    0,
    0,
    NULL,
    0,
    0
);
