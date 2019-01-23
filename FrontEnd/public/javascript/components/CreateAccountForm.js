

export default ({onCreateAccount=f=>f,error=""})=>{
    let newUsername,
        newPass,
        newPassRepeat

    return(
    <div className="createAccount center subPage">
        <form onSubmit={e=>onCreateAccount(e,newUsername,newPass,newPassRepeat)}>
                <div className="subText">Password and username must be 6 characters long and contain only alphanumeric characters.</div>
               <div className="subTitle">Username:</div>
                <input type="text" ref={ele=>newUsername=ele}/>
                <div className="subTitle">Password:</div>
                <input type="password" ref={ele=>newPass=ele}/>
                <div className="subTitle">Password Again:</div>
                <input type="password" ref={ele=>newPassRepeat=ele}/>
                <button>Submit</button>
        </form>
        <div className="error">{error}</div>
    </div>
    )
}