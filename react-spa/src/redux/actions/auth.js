import {AUTH, GOOGLE_IP, MICROSOFT_IP} from "../const/actionsTypes"

import axios from 'axios';
import User from "../../models/user"

export const loadUser = () => async (dispath)=>{
    console.log("auth.loadUser");
    
    const localUser = JSON.parse(localStorage.getItem("user_info"))

    if(localUser){
        dispath({type: AUTH, data: localUser})
    }
}

export const signin = (data, navigate) => async (dispath) =>{
    console.log("auth.signin");

    try{
        navigate("/")
    }catch(err){
        console.log(err);
    }
}

export const signinGoogle = (accessToken, navigate) => async (dispatch)=>{
    console.log("auth.signinGoogle");

    try{
        axios
            .get("https://www.googleapis.com/oauth2/v3/userinfo", {
            headers: {
                "Authorization": `Bearer ${accessToken}`
            }
            })
            .then(async response => {
                // Extract user information from the response
                const { given_name: firstName, family_name: lastName, email, picture } = response.data;
                
                console.log("firstName: ", firstName);
                console.log("lastName: ", lastName);
                console.log("email: ", email);
                console.log("picture: ", picture); 

                const user = new User(firstName, lastName, email, picture);

                dispatch({type : AUTH, identityProviderType: GOOGLE_IP, data: user})
                navigate("/")
            })
            .catch(err => {
                console.log(`Invalid access token! Error: ${err}`);
            })

        navigate("/")
    }catch(err){
        console.log(err);
    }
}

export const signinMicrosoft = (response, navigate) => async (dispatch)=>{
    console.log("auth.signinMicrosoft");

    try{
        console.log(`Response:${response.accessToken}`);

        const user = {}
        
        dispatch({type : AUTH, identityProviderType: MICROSOFT_IP, data: user})
        navigate("/")
    }catch(err){
        console.log(err);
    }
}

export const signup = (formData, navigate) => async (dispatch)=>{
    console.log("auth.signup");

    try{
        navigate("/")
    }catch(err){
        console.log(err);
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