

export default ({onLogin=f=>f,togglePage=f=>f,loginError="",show=true},Hideable)=>{
    let uname
    let pass
    return (
     <div className="loginBox center subPage">
        <div className="mainTitle">Best Bank Ledger</div>
        <form onSubmit={(e)=>onLogin(e,uname.value,pass.value)}>
            <div className="subTitle">Username</div>
            <input type="text" maxLength="30" ref={ele=>uname=ele} id="usernameInput"/>
            <div className="subTitle">Password</div>
            <input type="password" maxLength="30" ref={ele=>pass=ele} id="passwordInput"/>
            <button>Submit</button>
        </form>
        <button className="createAccount" onClick={(e)=>togglePage("create_account")}>Create New Account</button>
        <div className="loginError">{loginError}</div>
    </div>
    );
}


