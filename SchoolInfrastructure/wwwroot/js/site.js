/* Стилі для аватара */
.avatar {
    font - weight: bold;
    color: #333;
}

/* Стилі для навігації */
nav ul {
    list - style: none;
    padding: 0;
    display: flex;
    gap: 20px;
}

nav ul li a {
    text - decoration: none;
    color: #007bff;
    font - weight: 500;
}

nav ul li a:hover {
    color: #0056b3;
}

/* Адаптивність */
@media(max - width: 768px) {
    nav ul {
        flex - direction: column;
        gap: 10px;
    }
}