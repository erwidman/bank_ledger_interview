
const setError = s=>
    (s && s.includes("Failed")) ? "isError" : "notError"


export default({username="",onHistory=f=>f,onDeposit=f=>f,onWithdraw=f=>f,mainMsg="",onLogout=f=>f,currentBalance})=>{
    let depositAmt,
        withdrawAmt;

    return(
    <div className="subPage center mainPage">
      
        <div className="messages">
            <div className="currentBalance">Current Balance <br/>
                ${currentBalance.toFixed(2)}
            </div>
            <div className="mainMsg" id={setError(mainMsg)}>{mainMsg}</div>
        </div>
        <div className="bankForms">
            <form onSubmit={e=>onDeposit(e,depositAmt)}>
                <input type="number" min="0.01" step="0.01" max="1000000" placeholder="enter deposit" ref={ele=>depositAmt=ele}/>
                <button>Submit Deposit</button>
            </form>
            <form onSubmit={e=>onWithdraw(e,withdrawAmt)}>
                <input type="number" min="0.01" step="0.01" max="1000000" placeholder="enter withdraw"  ref={ele=>withdrawAmt=ele}/>
                <button>Submit Withdraw</button>
        </form>
        </div>
        <div className="bottomObjects">
            <button className="logOut" onClick={()=>document.location.reload()}>Logout</button>
            <button className="showHistory" onClick={onHistory}>Show History</button>
            
        </div>
    </div>
    )
}