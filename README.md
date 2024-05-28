1. I have added authentication and authorization to the project so before using it register user and then login.
2. I have added create,edit,delete and details link to do CRUD operations.
3. I have used ASP.Net core MVC, Entity Framework Core, MSSQL server.
4. I have added migration to the project and you don't need to update database from package manager console that is handled by the code itself so just run the project.
5. I have added one validation to the model that circular dependency is not allowed such as abc is parent of def and def is parent of ghi and ghi is parent of jkl , then abc can't have any def or ghi or jkl as its parent,
this is not allowed also abc can't be parent of itself.
6.  Also when you delete a subdepartment or department, so if that department A is parent of any subdepartment then from all the subdepartments that department parent A is removed, and if the department A is subdepartment of any
other department then from that department subdepartment list department A is removed, and then finally department A is deleted.

