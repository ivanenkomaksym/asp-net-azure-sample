class User {
    constructor(firstName, lastName, email, picture, identityProvider, token) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
        this.picture = picture;
        this.identityProvider = identityProvider;
        this.token = token;
    }
}

// Export the User class
module.exports = User;