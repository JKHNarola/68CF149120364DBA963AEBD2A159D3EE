namespace App.BL
{
    public enum Role
    {
        Admin = 1,
        WebUser = 2,
        System = 3,
        Dev = 4
    }

    public enum Operator
    {
        Gt,
        Lt,
        Eq,
        Le,
        Ge,
        Ne,
        Contains,
        StartsWith,
        EndsWith,
        In
    }

    public enum SortOrder
    {
        Asc,
        Desc
    }
}
