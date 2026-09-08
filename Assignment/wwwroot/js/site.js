
$(document).ready(function () {

    $("#sortId").click(function () {

        var rows = $("#userTable tbody tr").get();

        rows.sort(function (a, b) {

            var idA = Number($(a).find("td:eq(0)").text().trim());
            var idB = Number($(b).find("td:eq(0)").text().trim());

            return idA - idB;
        });

        $.each(rows, function (index, row) {
            $("#userTable tbody").append(row);
        });

    });

});

$(document).ready(function () {

    $("#sortPhone").click(function () {

        var rows = $("#userTable tbody tr").get();

        rows.sort(function (a, b) {

            var phoneA = Number($(a).find("td:eq(2)").text().trim());
            var phoneB = Number($(b).find("td:eq(2)").text().trim());

            return phoneA - phoneB;
        });

        $.each(rows, function (index, row) {
            $("#userTable tbody").append(row);
        });

    });

});

$(document).ready(function () {

    $("#sortName").click(function () {

        var rows = $("#userTable tbody tr").get();

        rows.sort(function (a, b) {

            var nameA = $(a).find("td:eq(1)").text().trim();
            var nameB = $(b).find("td:eq(1)").text().trim();

            return nameA.localeCompare(nameB);
        });

        $.each(rows, function (index, row) {
            $("#userTable tbody").append(row);
        });

    });

});

$(document).ready(function () {

    $("#sortLocation").click(function () {

        var rows = $("#userTable tbody tr").get();

        rows.sort(function (a, b) {

            var locationA = $(a).find("td:eq(3)").text().trim();
            var locationB = $(b).find("td:eq(3)").text().trim();

            return locationA.localeCompare(locationB);
        });

        $.each(rows, function (index, row) {
            $("#userTable tbody").append(row);
        });

    });

});

function togglePassword(button) {

    var password = button.nextElementSibling;
    var icon = button.querySelector("img");

    if (password.style.display === "none") {

        password.style.display = "inline";
        icon.src = "/images/close_eye.jpg";
        icon.alt = "Hide password";

    } else {

        password.style.display = "none";
        icon.src = "/images/open_eye.jpg";
        icon.alt = "Show password";

    }
}

    $(document).ready(function () {

        var rowsPerPage = 10;
        var currentPage = 1;

        var rows = $("#userTableBody tr");

        var totalPages = Math.ceil(rows.length / rowsPerPage);

        function showPage(page) {

            currentPage = page;

            rows.hide();

            var start = (page - 1) * rowsPerPage;
            var end = start + rowsPerPage;

            rows.slice(start, end).show();

            createPagination();
        }

        function createPagination() {

            var pagination = $("#pagination");

            pagination.empty();

            // Previous button
            if (currentPage > 1) {
                pagination.append(
                    '<button class="page-button" data-page="' +
                    (currentPage - 1) +
                    '">&lt;</button>'
                );
            }

            // Page numbers
            for (var i = 1; i <= totalPages; i++) {

                var activeClass =
                    i === currentPage ? " active" : "";

                pagination.append(
                    '<button class="page-button' +
                    activeClass +
                    '" data-page="' +
                    i +
                    '">' +
                    i +
                    '</button>'
                );
            }

            // Next button
            if (currentPage < totalPages) {
                pagination.append(
                    '<button class="page-button" data-page="' +
                    (currentPage + 1) +
                    '">&gt;</button>'
                );
            }
        }

        // Page button click
        $(document).on("click", ".page-button", function () {

            var page = parseInt($(this).attr("data-page"));

            showPage(page);
        });

        // Start on page 1
        showPage(1);

    });


    const userDropdownButton =
    document.getElementById("userDropdownButton");

    const userDropdownMenu =
    document.getElementById("userDropdownMenu");


    if (userDropdownButton && userDropdownMenu) {

        userDropdownButton.addEventListener(
            "click",
            function (event) {

                event.stopPropagation();

                userDropdownMenu.classList.toggle("show");

            }
        );


    document.addEventListener(
    "click",
    function () {

        userDropdownMenu.classList.remove("show");

            }
    );

    }
