(async () => {
    const body = document.getElementById('body');

    body.addEventListener("click", (e) => {
        if ($(e.target).hasClass('btn-add-item'))
            addItem(e.target);
        else if ($(e.target).hasClass('btn-remove-quantifiable'))
            removeQuantifiableItem(e.target);
        else if ($(e.target).hasClass('btn-remove-describable'))
            removeDescribableItem(e.target);
        else
            console.log("non functioning: " + e.target);
    });

    function addItem(element) {
        var buttonId = $(element).attr("id");
        console.log("/Characters/" + buttonId.replace("btn", ""));

        $.ajax({
            async: true,
            data: $("#form").serialize(),
            type: "POST",
            url: "/Characters/" + buttonId.replace("btn", ""),
            success: function (partialView) {
                var containerName = "#" + buttonId
                    .replace("btn", "")
                    .replace("Add", "")
                    .replace("y", "ie")
                    .toLowerCase() + "sContainer";
                console.log(containerName);
                $(containerName).html(partialView);
            }
        });
    }

    function removeQuantifiableItem(element) {
        //  get the current item index
        var strIndex = $(element).parent().children()[0]
            .getAttribute('for')
            .split("_")[1];
        var index = parseInt(strIndex);

        //console.log(strIndex, index);

        //  get the list container and number of items in list
        var listContainer = $(element).parent().parent().parent();
        var itemCount = listContainer.children().length;

        console.log(listContainer.children(), itemCount);

        //  Reset the indecies of all subsequent items
        for (var i = ++index; i < itemCount; i++) {
            var currentItem = listContainer.children('div')[i].childNodes[1];

            console.log(currentItem);

            decrementAttrIndex(currentItem.childNodes[1], 'for', '_');
            decrementAttrIndex(currentItem.childNodes[3], 'name', '[');
            decrementAttrIndex(currentItem.childNodes[5], 'for', '_');
            decrementAttrIndex(currentItem.childNodes[7], 'id', '_');
            decrementAttrIndex(currentItem.childNodes[7], 'name', '[');
        }

        //  Remove the current item in a list
        element.parentNode.parentNode.remove();
    }

    function removeDescribableItem(element) {
        //  get the current item index
        var strIndex = $(element).parent().children()[0]
            .getAttribute('for')
            .split("_")[1];
        var index = parseInt(strIndex);

        //console.log(strIndex, index);

        //  get the list container and number of items in list
        var listContainer = $(element).parent().parent().parent();
        var itemCount = listContainer.children().length;

        console.log(listContainer.children(), itemCount);

        //  Reset the indecies of all subsequent items
        for (var i = ++index; i < itemCount; i++) {
            var currentItem = listContainer.children('div')[i].childNodes[1];

            console.log(currentItem);

            decrementAttrIndex(currentItem.childNodes[1], 'for', '_');
            decrementAttrIndex(currentItem.childNodes[3], 'id', '_');
            decrementAttrIndex(currentItem.childNodes[3], 'name', '[');

            currentItem = listContainer.children('div')[i].childNodes[3];
            console.log(currentItem);

            decrementAttrIndex(currentItem.childNodes[1], 'for', '_');

            currentItem = currentItem.childNodes[3]
            console.log(currentItem);

            decrementAttrIndex(currentItem.childNodes[1], 'id', '_');
            decrementAttrIndex(currentItem.childNodes[1], 'name', '[');
        }

        element.parentNode.parentNode.remove();
    }

    function decrementAttrIndex(ele, attr, delimiter) {
        console.log(ele, attr, delimiter);

        var value = ele.getAttribute(attr);
        var index = parseInt(value.split(delimiter)[1]);
        var newIndex = index - 1;
        ele.setAttribute(attr, value.replace(index.toString(), newIndex.toString()));

        console.log(value, index, newIndex, ele.getAttribute(attr));
    }
})();