import React, { Component } from 'react'
import Rest from '../util/RestInterface.js'
import validator from 'validator'

import LoginBox from './LoginBox.js'
import CreateAccountForm from './CreateAccountForm.js'
import MainPage from './MainPage.js'
import {MakeHideable} from '../animations/Animations.js'


const validateUsername = (uname)=>
    validator.isAlphanumeric(uname) && uname.length >= 6

const validatePassword = (pass,pass1)=>
    validator.isAlphanumeric(pass) && validator.isAlphanumeric(pass1) && pass === pass1 && pass.length >= 6

const parseAmt = (amount)=>{
    let amt = amount.value
    amt = Number(amt)
    amt.toFixed(2)
    return amt
}





export default class MainBox extends Component {
    
    constructor(props){
        super(props)
        this.state = {
            loginError : "",
            createAccountError: "",
            page : "login",
            mainError : "",
            mainMsg : "",
            currentBalance : 0
        }   
    }

    onLogin = (e,uname,pass)=>{
        e.preventDefault();
        this.togglePage("loading");
        Rest.setAuth(uname,pass)
        .then(()=>Rest.getCurrentAmount())
        .then((amt)=>{
            console.log(amt)
            this.setState({...this.state,
                            loginError:"",
                            page : "main_page",
                            currentBalance : amt.data
                        })
        })
        .catch((err)=>{
            this.togglePage('login');
            if(err.response && err.response.status==401){
                this.setState({...this.state,
                                loginError:"Invalid Login!",})
            }
            else
                this.setState({...this.state,
                                loginError:"Error with service!"})
        })
    }

    onCreateAccount = (e,uname,pass,pass2)=>{
        e.preventDefault()
        let userValid = validateUsername(uname.value)
        let passValid = validatePassword(pass.value,pass2.value)
        if(userValid && passValid){
            this.togglePage('loading');
            Rest.createAccount(uname.value,pass.value)
            .then((pass)=>{

                if(pass){
                    this.togglePage('account_created');
                setTimeout(()=>this.togglePage('login'),2000);
                }
                uname.value = "";
                pass.value = "";
            })
            .catch((err)=>{
                this.setState({...this.state,createAccountError:"Username taken or offline."});
            })
           
        }
        else {
            if(!userValid && !passValid)
                this.setState({...this.state,createAccountError:"Invalid username and password."})
            else if(!userValid)
                this.setState({...this.state,createAccountError:"Invalid username."})
            else
                this.setState({...this.state,createAccountError:"Invalid pass."})
        }
        pass.value="";
        pass2.value ="";
    }

    onAlter = (API,amount,type,pastTense,getAmount) =>{
        this.togglePage('loading')
        let amt= parseAmt(amount);
        API(amt)
        .then(()=>getAmount())
        .then((newamt)=>{
            this.setState({
                            ...this.state,
                            mainError: "",
                            currentBalance : newamt.data,
                            mainMsg:`${pastTense} $${amt}!`,
                            page:'main_page'
                        })
        })
        .catch((err)=>{
            this.setState({
                            ...this.state,
                            mainError:`${type} Failed!`,
                            page : "main_page"
                        })
        });
        amount.val="";

    }

    onWithdraw = (e,amount) =>{
        e.preventDefault()
        this.onAlter(Rest.withdraw,amount,"Withdraw","Withdrew",Rest.getCurrentAmount)
        amount.value = ""

    }
    onDeposit = (e,amount) =>{
        e.preventDefault()
        this.onAlter(Rest.deposit,amount,"Deposit","Deposited",Rest.getCurrentAmount)
        amount.value=""
    }

    togglePage = (page)=>
        this.setState(
            {
                ...this.state,
                page,
                createAccountError:"",
                loginError:""
            })


    

    render() {
        return (
            <div className="mainBox">
            {
                MakeHideable(
                      <LoginBox
                        onLogin={this.onLogin}
                        togglePage={this.togglePage}
                        loginError={this.state.loginError}
                     />,
                    this.state.page,
                    "login"
                )
            }
            {
                MakeHideable(
                    <CreateAccountForm 
                        onCreateAccount={this.onCreateAccount}
                        error={this.state.createAccountError}
                    />,
                    this.state.page,
                    "create_account"
                )       
            }
            {
                MakeHideable(
                    <div className="blockBox">
                        <div className="center">Account Made</div>
                    </div>,
                    this.state.page,
                    "account_created"
                )
            }
            {
                MakeHideable(
                    <div className="blockBox">
                        <div className="center">Loading...</div>
                    </div>,
                    this.state.page,
                    "loading"
                )
            }
            {
                MakeHideable(
                    
                    <MainPage 
                        onDeposit={this.onDeposit}
                        onWithdraw={this.onWithdraw}
                        error={this.state.mainError}
                        mainMsg={this.state.mainMsg}
                        currentBalance ={this.state.currentBalance}
                    />,
                    this.state.page,
                    "main_page"
                )
            }
            </div>
        );
    }
}
