export default({username="",onHistory=f=>f,onDeposit=f=>f,onWithdraw=f=>f,error="",mainMsg="",currentBalance})=>{
    let depositAmt,
        withdrawAmt;

    return(
    <div className="subPage center mainPage">
        <div className="currentBalance">Current Balance <br/>
            ${currentBalance.toFixed(2)}
        </div>
        <div className="bankForms">
            <form onSubmit={e=>onDeposit(e,depositAmt)}>
                <div className="subTitle">Deposit</div>
                <input type="number" min="0.01" step="0.01" max="1000000" placeholder="enter $$$" ref={ele=>depositAmt=ele}/>
                <button>Submit Deposit</button>
            </form>
            <form onSubmit={e=>onWithdraw(e,withdrawAmt)}>
                <div className="subTitle"> Withdraw</div>
                <input type="number" min="0.01" step="0.01" max="1000000" placeholder="enter $$$"  ref={ele=>withdrawAmt=ele}/>
                <button>Submit Withdraw</button>
        </form>
        </div>
        <div className="errorMsg">{error}</div>
        <div className="mainMsg">{mainMsg}</div>
    </div>
    )
}