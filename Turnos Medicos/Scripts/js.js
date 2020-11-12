jQuery.extend(jQuery.validator.messages, {
    required: "Este campo es requerido.",
    remote: "Por favor, arregle este campo.",
    email: "Por favor, introduzca una dirección de correo electrónico válida.",
    url: "Por favor, introduzca una URL válida.",
    date: "Por favor, introduzca una fecha válida.",
    dateISO: "Por favor, introduzca una fecha válida (ISO).",
    number: "Por favor, introduzca un número válido.",
    digits: "Por favor, introduzca sólo dígitos.",
    creditcard: "Por favor, introduzca un número válido de tarjeta de crédito.",
    equalTo: "Por favor, introduzca el mismo valor de nuevo.",
    accept: "Por favor, introduzca un valor con una extensión válida.",
    maxlength: jQuery.validator.format("Por favor, introduzca no más de {0} caracteres."),
    minlength: jQuery.validator.format("Por favor, introduzca al menos {0} caracteres."),
    rangelength: jQuery.validator.format("Por favor, introduzca un valor entre {0} y {1}caracteres de largo."),
    range: jQuery.validator.format("Por favor, introduzca un valor entre {0} y {1}."),
    max: jQuery.validator.format("Por favor, introduzca un valor menor o igual a {0}."),
    min: jQuery.validator.format("Por favor, introduzca un valor mayor o igual a {0}.")
});