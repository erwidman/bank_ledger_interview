export default ({history=[],onGoBack=f=>f})=>{
    if(!history)
        history=[]
    return(
    <div className="subPage center historyPage">
        <button className="goBack" onClick={onGoBack}>Back</button>
        <table className="historyTable">
        <thead>
            <tr>
                <th>Action</th>
                <th>Amount</th>
                <th>Time</th>
            </tr>
        </thead>
        <tbody>
            {
                history.map((row,i)=>
                <tr className={i%2==0?"even":"odd"} key={i}>
                    <td>{row.action}</td>
                    <td>${row.delta}</td>
                    <td>{row.time}</td>
                </tr>
                 )
            }
        </tbody>
        </table>
        <div className="subText">(Last {history.length} actions)</div>
    </div>
    )

}