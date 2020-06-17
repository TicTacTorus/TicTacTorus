function custom_alert(title, message)
{
    //customize?
    let str = "" + title+"!";
    if(str.length > 0)
    {
        str += "\n";
    }
    str += message;
    alert(str);
    return true;
}