import {AUTH, GOOGLE_IP, MICROSOFT_IP, ORGANIZATION} from "../const/actionsTypes"

import { callMsGraph } from '../../graph';
import * as api from "../../api/index"

import axios from 'axios';
import User from "../../models/user"

export const loadUser = () => async (dispath)=>{
    console.log("auth.loadUser");
    
    const localUser = JSON.parse(localStorage.getItem("user_info"))

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

    const user = new User(firstName, lastName, email, picture, GOOGLE_IP, id_token);

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

            const user = new User(firstName, lastName, email, null, MICROSOFT_IP, response.accessToken);

            dispatch({type : AUTH, data: user})
            navigate("/")
        });
    }catch(err){
        console.log(err);
    }
}

export const signinOrg = (id_token, navigate) => async (dispatch)=>{
    console.log("auth.signinOrg");

    try{
        const user = new User("TODO", "TODO", "TODO", null, ORGANIZATION, id_token);

        dispatch({type : AUTH, data: user})
        navigate("/")
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