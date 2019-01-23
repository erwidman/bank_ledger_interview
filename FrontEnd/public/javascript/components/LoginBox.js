

export default ({onLogin=f=>f,togglePage=f=>f,loginError="",show=true},Hideable)=>{
    let uname
    let pass
    //console.log(toggleState)
    return (
     <div className="loginBox center subPage">
        <div className="mainTitle">Best Bank Ledger (BBL)</div>
        <form onSubmit={(e)=>onLogin(e,uname.value,pass.value)}>
            <div className="subTitle">Username</div>
            <input type="text" ref={ele=>uname=ele} id="usernameInput"/>
            <div className="subTitle">Password</div>
            <input type="password" ref={ele=>pass=ele} id="passwordInput"/>
            <button>Submit</button>
        </form>
        <button className="createAccount" onClick={(e)=>togglePage("create_account")}>Create New Account</button>
        <div className="loginError">{loginError}</div>
    </div>
    );
}


