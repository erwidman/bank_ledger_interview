import axios from 'axios'
import {endpoint} from "./Constants.js"

axios.defaults.headers.post['Content-Type'] = 'application/json;charset=utf-8';
axios.defaults.headers.post['Access-Control-Allow-Origin'] = '*';


var auth;




const setAuth = (username,password)=>{
    return axios.post(
        `${endpoint}/api/Login`,
        {},
        {
            auth : {
                username,
                password
            },
        }
    )
    .then((response)=>{
        if(response.status==200){
            auth = {username,password};
            return true;
        }
        return false;
    });
};

const logout = ()=>{
    if(!auth)
        return false;
    auth = null;
    return true;
}


const alter = (type,amt)=>{
     return axios.post(
        `${endpoint}/api/Alter`,
        {
            action : type,
            amount : amt
        },
        {
            auth 
        }
    )
}


const deposit = (amt)=>{
    if(typeof amt != "number")
        throw "invalid_amount"
    if(!auth)
        throw "noAuth"
    amt = amt.toFixed(2);
    return alter("deposit",amt)

}

const withdraw = (amt)=>{
    if(typeof amt != "number")
        throw new Error("invalid_amount");
    if(!auth)
        throw new Error("noAuth");
    amt = amt.toFixed(2);
    return alter("withdraw",amt)
}

const getHistory = ()=>{
    if(!auth)
        return "noAuth";
    axios.post(
        `${endpoint}/api/History`,
        {},{auth}
    )
    .then((data)=>{
        console.log(JSON.parse((data.data)));
    });
}

const getCurrentAmount = ()=>{
    return axios.post(
        `${endpoint}/api/CurrentAmount`,
        {},{auth}
    )
}

const createAccount = (username,password)=>{
    return axios.post(
        `${endpoint}/api/CreateAccount`,
        {},
        {
            auth : {
                username,
                password
            },
        }
    )
    .then((response)=>{
        if(response.status==200){
            auth = {username,password};
            return true;
        }
        return false;
    });
}


export default {
                setAuth,
                deposit,
                withdraw,
                getHistory,
                createAccount,
                getCurrentAmount
            };