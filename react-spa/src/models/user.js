class User {
    constructor(firstName, lastName, email, picture, identityProvider, id_token, refresh_token) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
        this.picture = picture;
        this.identityProvider = identityProvider;
        this.id_token = id_token;
        this.refresh_token = refresh_token;
    }
}

// Export the User class
module.exports = User;