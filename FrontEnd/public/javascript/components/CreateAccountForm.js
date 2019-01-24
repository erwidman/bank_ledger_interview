

export default ({onCreateAccount=f=>f,error="",onBack=f=>f})=>{
    let newUsername,
        newPass,
        newPassRepeat

    return(
    <div className="createAccount center subPage">
        <form onSubmit={e=>onCreateAccount(e,newUsername,newPass,newPassRepeat)}>
                <div className="subText">Password and username must be 6 to 30 characters long and contain only alphanumeric characters.</div>
               <div className="subTitle">Username:</div>
                <input type="text" maxLength="30" ref={ele=>newUsername=ele}/>
                <div className="subTitle">Password:</div>
                <input type="password" maxLength="30" ref={ele=>newPass=ele}/>
                <div className="subTitle">Password Again:</div>
                <input type="password"  maxLength="30" ref={ele=>newPassRepeat=ele}/>
                <button>Submit</button>
                <button onClick={onBack}>Back</button>
        </form>
        <div className="error">{error}</div>
    </div>
    )
}