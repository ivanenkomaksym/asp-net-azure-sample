import {AUTH, GOOGLE_IP, MICROSOFT_IP, ORGANIZATION, AUTH0} from "../const/actionsTypes"

import { callMsGraph } from '../../graph';
import * as api from "../../api/index"

import axios from 'axios';
import User from "../../models/user"

import { jwtDecode } from 'jwt-decode';
import { loadUserFromLocalStorage } from "../reducers/auth";
import { LOGOUT } from "../../redux/const/actionsTypes"

export const loadUser = () => async (dispath)=>{
    console.log("auth.loadUser");
    
    const localUser = loadUserFromLocalStorage();

    if(localUser){
        dispath({type: AUTH, data: localUser})
    }
}

export const signin = (input, navigate, errorHandler) => async (dispath) =>{
    console.log("auth.signin");

    try{
        console.log("input:", JSON.stringify(input, null, 2));
        const {data} = await api.signIn(input)

        dispath({type: AUTH, data})
        navigate("/")
    }catch(err){
        errorHandler(err);
    }
}

export const signinGoogleWithAccessToken = (accessToken, navigate) => async (dispatch)=>{
    console.log("auth.signinGoogleWithAccessToken");

    try{
        axios
            .get("https://www.googleapis.com/oauth2/v3/userinfo", {
            headers: {
                "Authorization": `Bearer ${accessToken}`
            }
            })
            .then(async response => {
                handleGoogleResponse(response, null, dispatch, navigate);
            })
            .catch(err => {
                console.log(`Invalid access token! Error: ${err}`);
            })

        navigate("/")
    }catch(err){
        console.log(err);
    }
}

export const signinGoogleWithIdToken = (id_token, navigate) => async (dispatch)=>{
    console.log("auth.signinGoogleWithIdToken");

    try{
        axios
            .get(`https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=${id_token}`)
            .then(async response => {
                console.log("GoogleWithIdToken response: ", JSON.stringify(response, null, 2));
                handleGoogleResponse(response, id_token, dispatch, navigate);
            })
            .catch(err => {
                console.log(`Invalid access token! Error: ${err}`);
            })

        navigate("/")
    }catch(err){
        console.log(err);
    }
}

function handleGoogleResponse(response, id_token, dispatch, navigate) {
    // Extract user information from the response
    const { given_name: firstName, family_name: lastName, email, picture } = response.data;

    console.log("firstName: ", firstName);
    console.log("lastName: ", lastName);
    console.log("email: ", email);
    console.log("picture: ", picture);

    const user = new User(/*firstName       */firstName,
                          /*lastName        */lastName,
                          /*email           */email,
                          /*picture         */picture,
                          /*identityProvider*/GOOGLE_IP,
                          /*id_token        */id_token,
                          /*refresh_token   */null,
                          /*organization    */null);

    dispatch({ type: AUTH, data: user });
    navigate("/");
}

export const signinMicrosoft = (response, navigate) => async (dispatch)=>{
    console.log("auth.signinMicrosoft");

    try{
        console.log(`Response:${response.accessToken}`);

        callMsGraph(response.accessToken).then((response) => {
            console.log("callMsGraph response:", JSON.stringify(response, null, 2));
            const { givenName: firstName, surname: lastName, userPrincipalName: email } = response;
                
            console.log("firstName: ", firstName);
            console.log("lastName: ", lastName);
            console.log("email: ", email);

            const user = new User(/*firstName       */firstName,
                                  /*lastName        */lastName,
                                  /*email           */email,
                                  /*picture         */null,
                                  /*identityProvider*/MICROSOFT_IP,
                                  /*id_token        */response.accessToken,
                                  /*refresh_token   */null,
                                  /*organization    */null);

            dispatch({type : AUTH, data: user})
            navigate("/")
        });
    }catch(err){
        console.log(err);
    }
}

export const signinAuth0 = (auth0user, id_token, navigate) => async (dispatch)=>{
    console.log("auth.signinAuth0");

    try{
        const user = new User(/*firstName       */auth0user.given_name || auth0user.name?.split(' ')[0] || '',
                              /*lastName        */auth0user.family_name || auth0user.name?.split(' ').slice(1).join(' ') || '',
                              /*email           */auth0user.email,
                              /*picture         */auth0user.picture,
                              /*identityProvider*/AUTH0,
                              /*id_token        */id_token,
                              /*refresh_token   */null,
                              /*organization    */null);

        dispatch({type : AUTH, data: user})
        navigate("/")
    }catch(err){
        console.log(err);
    }
}

export const signinOrg = (id_token, refresh_token, navigate) => async (dispatch)=>{
    console.log("auth.signinOrg");

    try{
        const decodedToken = jwtDecode(id_token);
        console.log(decodedToken);

        const localUser = loadUserFromLocalStorage();
      
        // Example of extracting user info
        const firstName = decodedToken.given_name;
        const lastName = decodedToken.family_name;
        const email = decodedToken.email || decodedToken.unique_name;
        const profilePicture = decodedToken.picture;

        const user = new User(/*firstName       */firstName,
                              /*lastName        */lastName,
                              /*email           */email,
                              /*picture         */profilePicture,
                              /*identityProvider*/ORGANIZATION,
                              /*id_token        */id_token,
                              /*refresh_token   */refresh_token,
                              /*organization    */localUser.organization);

        dispatch({type : AUTH, data: user})
    }catch(err){
        console.log(err);
    }
}

export const signup = (formData, navigate, errorHandler) => async (dispatch)=>{
    console.log("auth.signup");

    try{
        // signup user
        const {data} = await api.signUp(formData)
        console.log("input:", JSON.stringify(data, null, 2));

        dispatch({type : AUTH, data})
        navigate("/")
    }catch(err){
        errorHandler(err);
    }
}

export const signupGoogle = (accessToken, navigate) => async (dispatch)=>{
    console.log("auth.signupGoogle");

    try{
        navigate("/")
    }catch(err){
        console.log(err);
    }
}

export const logout = () => async (dispatch) => {
    console.log("auth.logout");
    
    try {
        // Dispatch the LOGOUT action to clear Redux state and localStorage
        dispatch({ type: LOGOUT });
    } catch (err) {
        console.log("Logout error:", err);
    }
}