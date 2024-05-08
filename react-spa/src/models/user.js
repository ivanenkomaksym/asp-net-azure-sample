class User {
    constructor(firstName, lastName, email, picture, identityProvider) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
        this.picture = picture;
        this.identityProvider = identityProvider;
    }
}

// Export the User class
module.exports = User;