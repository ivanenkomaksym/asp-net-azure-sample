class User {
    constructor(firstName, lastName, email, picture, identityProvider, id_token, refresh_token, organization) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
        this.picture = picture;
        this.identityProvider = identityProvider;
        this.id_token = id_token;
        this.refresh_token = refresh_token;
        this.organization = organization;
    }
}

// Export the User class
module.exports = User;