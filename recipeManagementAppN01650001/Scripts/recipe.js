window.onload = function () {
    if (document.getElementById("addIngredient")) {
        document.getElementById("addIngredient").addEventListener("click", function () {
            var ingredientList = document.getElementById("ingredientsList");
            var count = ingredientList.children.length;
            var newIngredient = document.createElement("div");
            newIngredient.classList.add("row", "mb-2", "ingredient-item");
            newIngredient.innerHTML = `
                <div class="col d-flex justify-content-center align-items-center gap-2">
                    <label>Name:</label>
                    <input type="text" class="form-control" name="Ingredients[${count}].IngredientName" />
                </div>
                <div class="col d-flex justify-content-center align-items-center gap-2">
                    <label>Quantity:</label>
                    <input type="number" class="form-control" name="Ingredients[${count}].IngredientQuantity" />
                </div>
                <div class="col d-flex justify-content-center align-items-center gap-2">
                    <label>Unit:</label>
                    <input type="text" class="form-control" name="Ingredients[${count}].IngredientUnit" />
                </div>
                <div class="col-1">
                    <button type="button" class="btn btn-danger remove-ingredient">-</button>
                </div>
            `;
            ingredientList.appendChild(newIngredient);
        });
    }
    if (document.getElementById("addInstruction")) {
        document.getElementById("addInstruction").addEventListener("click", function () {
            var instructionList = document.getElementById("instructionsList");
            var count = instructionList.children.length;
            var newInstruction = document.createElement("div");
            newInstruction.classList.add("row", "mb-2", "instruction-item");
            newInstruction.innerHTML = `
                        <div class="col-11 d-flex align-items-center gap-2">
                            <label>Step ${count + 1}:</label>
                            <input type="hidden" name="Instructions[${count}].StepNumber" value="${count + 1}"/>
                            <input type="text" class="form-control" name="Instructions[${count}].Description" />
                            <button type="button" class="btn btn-danger remove-instruction">-</button>
                        </div>
                `;
            instructionList.appendChild(newInstruction);
        });
    }

    if (document.getElementById("addEditInstruction")) {
        document.getElementById("addEditInstruction").addEventListener("click", function () {
            var instructionList = document.getElementById("instructionsList");
            var count = instructionList.children.length;
            var newInstruction = document.createElement("div");
            newInstruction.classList.add("row", "mb-2", "instruction-item");
            newInstruction.innerHTML = `
                        <div class="col-11 d-flex align-items-center gap-2">
                            <input type="text" class="form-control w-25" name="Instructions[${count}].StepNumber" value="${count + 1}"/>
                            <input type="text" class="form-control" name="Instructions[${count}].Description" />
                            <button type="button" class="btn btn-danger remove-instruction">-</button>
                        </div>
                `;
            instructionList.appendChild(newInstruction);
        });
    }

    document.addEventListener("click", function (event) {
        if (event.target.classList.contains("remove-ingredient")) {
            event.target.closest('.ingredient-item').remove();
        } else if (event.target.classList.contains("remove-instruction")) {
            event.target.closest('.instruction-item').remove();
        }
    });

    if (document.getElementById("printRecipe")) {
        document.getElementById("printRecipe").addEventListener("click", function printRecipe() {
            var printContents = document.getElementById('recipe').innerHTML;
            console.log(printContents)
            var originalContents = document.body.innerHTML;

            document.body.innerHTML = printContents;
            window.print();
            document.body.innerHTML = originalContents;
        });
    }
}
