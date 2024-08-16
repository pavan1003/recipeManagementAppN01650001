# RecipeManagementApp

RecipeManagementApp is an application designed to help users manage their recipes, ingredients, and instructions efficiently. This project leverages ASP.NET MVC for the frontend, ASP.NET Web API for backend services, and Entity Framework for ORM.

## Features
- **Manage Recipes:** Create, Read, Update, and Delete recipe data.
- **Ingredient Management:** Manage ingredients linked to recipes.
- **Instruction Management:** Manage instructions linked to recipes.
- **Relational Data:** Handle relationships between recipes, ingredients, and instructions.

## Running this Project
### Prerequisites
- Visual Studio 2019 or later
- .NET Framework 4.7.2

### Setup Steps
1. **Clone the Repository:**
   ```bash
   git clone https://github.com/pavan1003/recipeManagementAppN01650001.git
   ```
   
2. **Open the Project:**
   Open the solution file `RecipeManagementApp.sln` in Visual Studio.

3. **Set Target Framework:**
   - Project > RecipeManagementApp Properties > Change target framework to 4.7.1 -> Change back to 4.7.2

4. **Create Database:**
   - Make sure there is an `App_Data` folder in the project (Right-click solution > View in File Explorer, then create the folder if it doesn't exist).
   - Tools > NuGet Package Manager > Package Manager Console > Run command: `Update-Database`
   - Ensure the database is created using:
     - View > SQL Server Object Explorer > MSSQLLocalDb > ..

5. **Run the Application:** 
   You can run the application using Visual Studio by hitting `F5` or selecting Debug > Start Debugging.

### Common Issues and Resolutions
- **Could not attach .mdf database:** Ensure `App_Data` folder is created.
- **Error: 'Type' cannot be null:** Update Entity Framework to the latest version.
- **Exception has been thrown by the target of an invocation:** Clone the project repository to the actual drive on the machine.
- **System.Web.Http does not have reference to serialize:** Add reference to `System.Web.Extensions`.

### Running API Commands
Configure and run API commands using tools like `curl` or Postman.

#### Ingredient Management
- **Get List of Ingredients:**
   ```bash
   curl https://localhost:44384/api/ingredientdata/listingredients
   ```
- **Get Single Ingredient:**
   ```bash
   curl https://localhost:44384/api/ingredientdata/findingredient/{id}
   ```
- **Add a New Ingredient:**
   ```bash
   curl -H "Content-Type:application/json" -d @ingredient.json https://localhost:44384/api/ingredientdata/addingredient
   ```
- **Delete an Ingredient:**
   ```bash
   curl -d "" https://localhost:44384/api/ingredientdata/deleteingredient/{id}
   ```
- **Update an Ingredient:**
   ```bash
   curl -H "Content-Type:application/json" -d @ingredient.json https://localhost:44384/api/ingredientdata/updateingredient/{id}
   ```

#### Recipe Management
- **Get List of Recipes:**
   ```bash
   curl https://localhost:44384/api/recipedata/listrecipes
   ```
- **Get Single Recipe:**
   ```bash
   curl https://localhost:44384/api/recipedata/findrecipe/{id}
   ```
- **Add a New Recipe:**
   ```bash
   curl -H "Content-Type:application/json" -d @recipe.json https://localhost:44384/api/recipedata/addrecipe
   ```
- **Delete a Recipe:**
   ```bash
   curl -d "" https://localhost:44384/api/recipedata/deleterecipe/{id}
   ```
- **Update a Recipe:**
   ```bash
   curl -H "Content-Type:application/json" -d @recipe.json https://localhost:44384/api/recipedata/updaterecipe/{id}
   ```

#### Instruction Management
- **Get List of Instructions:**
   ```bash
   curl https://localhost:44384/api/instructiondata/listinstructions
   ```
- **Get Single Instruction:**
   ```bash
   curl https://localhost:44384/api/instructiondata/findinstruction/{id}
   ```
- **Add a New Instruction:**
   ```bash
   curl -H "Content-Type:application/json" -d @instruction.json https://localhost:44384/api/instructiondata/addinstruction
   ```
- **Delete an Instruction:**
   ```bash
   curl -d "" https://localhost:44384/api/instructiondata/deleteinstruction/{id}
   ```
- **Update an Instruction:**
   ```bash
   curl -H "Content-Type:application/json" -d @instruction.json https://localhost:44384/api/instructiondata/updateinstruction/{id}
   ```

### Database Relationships
- **Recipe:** A recipe has many ingredients and many instructions.
- **Ingredient:** Each ingredient belongs to a recipe.
- **Instruction:** Each instruction belongs to a recipe.

### Table Relationship
- **Recipe Table:** `RecipeId` (Primary Key), `Title`, `Description`, `Category`, `CookingTime`
- **Ingredient Table:** `IngredientId` (Primary Key), `IngredientName`, `IngredientQuantity`, `IngredientUnit`, `RecipeId` (Foreign Key)
- **Instruction Table:** `InstructionId` (Primary Key), `StepNumber`, `Description`, `RecipeId` (Foreign Key)

Enjoy managing your recipes with RecipeManagementApp! For troubleshooting and more, refer to our detailed documentation and code comments.
