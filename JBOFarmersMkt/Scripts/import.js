﻿(function ($, _) {
    // Acts as a backup of the original html of the given element and returns
    // a function that will restore it when called.
    var reset_text_to_default = function (elem) {
        var html = $(elem).html();

        return function () {
            $(elem).html(html);
        };
    };

    var input_has_error = function (error_element, name, reason, template) {
        $(error_element).html(template({ name: name, reason: reason }))
        .show()
        .closest(".form-group")
        .removeClass("has-success")
        .addClass("has-error");
    };

    var input_has_success = function (error_element) {
        $(error_element).hide()
        .closest(".form-group")
        .removeClass("has-error")
        .addClass("has-success");
    };

    var display_preview = function (preview_element, template, dt_config) {
        return function (data, f) {
            $(preview_element).html(template({ rows: data, file: f })).find("table").dataTable(dt_config);
        };
    };

    // Disable form submission if there are any errors
    // or if the form is empty.
    var check_form = function () {
        // Default to disabled
        var disabled = true;

        // Check if both inputs are empty
        // See: http://stackoverflow.com/a/17044272
        var isEmpty = $("input[type=file]").filter(function () { return !this.value }).length == 2;

        var hasErrors = $("form").has(".has-error").length > 0;

        if (isEmpty || hasErrors) {
            disabled = true;
        } else {
            disabled = false;
        }

        $("form input[type='submit']").prop("disabled", disabled);
    };

    // Reset the given file input using the workaround found
    // at: http://stackoverflow.com/a/13351234
    // resetPreviewFn() should clear the preview area for the particular file
    var reset_file_input = function (elem, resetPreviewFn) {
        // Perform the workaround to reset the file input
        var e = $(elem);
        e.wrap('<form>').closest('form').get(0).reset();
        e.unwrap();

        // Remove any error or success classes since the file is no longer selected
        // Also hide any error messages that may exist
        e.closest('.form-group')
            .removeClass('has-error has-success')
            .find('.error-message')
            .hide();

        // The form could now be valid. Check it.
        check_form();

        // Reset the preview area since a file is no longer selected.
        resetPreviewFn();
    };

    // Takes a regex specifying a valid filename.
    // Returns a function that takes a file and compares its name to the regex
    // returning true or false as appropriate.
    var is_valid_filename = function (regex) {
        return function (file) {
            if (!file.name.match(regex)) {
                return false;
            }

            return true;
        }
    }

    var is_valid_product_filename = is_valid_filename(/stock_items.+csv$/);
    var is_valid_sales_filename = is_valid_filename(/sales_from_.+_to_.+csv$/);

    // Generic function for parsing the given file input when it changes. Takes care of adding the event handler, data validation, and error handling.
    var parse_file_on_change = function (file_input, error_element, error_template, resetFn, is_valid_filenameFn, displayFn) {
        var handle_error = function (name, reason) {
            input_has_error(error_element, name, reason, error_template);
            resetFn();
            check_form();
        };

        $(file_input).change(function () {
            var parse_results = $(this).parse({
                config: {
                    error: function (error, file) {
                        console.log("An error occurred with the chosen file: ", error);
                        handle_error(error.name, error.message);
                    },
                    complete: function (results, file) {
                        console.log(results);
                        input_has_success(error_element);
                        displayFn(results.data, file);
                        check_form();
                    }
                },
                before: function (file, input_element) {
                    // Make sure the file checks out before parsing and allowing the user to upload it.
                    a = { action: "continue" };
                    r = [];

                    if (!file.name.match(/.csv$/)) {
                        // The file must be a CSV. Abort parsing.
                        a = { action: "abort" };
                        r.push("Unsupported file selected. Only CSV files exported from ShopKeep are supported.");
                    }

                    if (!is_valid_filenameFn(file)) {
                        // The filename does not match that of ShopKeep exports.
                        a = { action: "abort" };
                        r.push("Wrong file selected. The name of the selected file is not consistent with filenames used by ShopKeep.");
                    }

                    a.reason = r;

                    return a;
                },
                // Called when there is an error parsing files
                error: function (error, file, input_element, reason) {
                    console.log("Error: ", error, " - ", reason);
                    handle_error(error.name, reason);
                }
            });
        });
    };

    $(function () {
        var table_template = _.template($("#table-template").html());
        var error_template = _.template($("#error-template").html());

        var reset_product_preview = reset_text_to_default("#product-preview");
        var reset_sales_preview = reset_text_to_default("#sales-preview");

        var display_products = display_preview("#product-preview", table_template,
            {
                "order": [1, "asc"],
                "searching": false,
                "lengthChange": false,
                "columnDefs": [
                    {
                        "targets": [3, 7, 8, 9, 11, 13, 14, 15, 17, 18, 19],
                        "visible": false
                    },
                    {
                        "targets": "_all",
                        "orderable": false
                    }
                ]
            });

        var display_sales = display_preview("#sales-preview", table_template,
            {
                "order": [1, "asc"],
                "searching": false,
                "lengthChange": false,
                "columnDefs": [
                    {
                        "targets": [3, 5, 6, 7, 8, 10, 14, 16],
                        "visible": false
                    },
                    {
                        "targets": "_all",
                        "orderable": false
                    }
                ]
            });

        $("#reset-products-input").click(function (e) { e.preventDefault(); reset_file_input("#products", reset_product_preview) });
        $("#reset-sales-input").click(function (e) { e.preventDefault(); reset_file_input("#sales", reset_sales_preview) });

        parse_file_on_change("input[type='file']#products", "#products-error", error_template, reset_product_preview, is_valid_product_filename, display_products);
        parse_file_on_change("input[type='file']#sales", "#sales-error", error_template, reset_sales_preview, is_valid_sales_filename, display_sales);
    });
})(jQuery, _);