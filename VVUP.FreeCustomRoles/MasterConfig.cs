using System.ComponentModel;
using Exiled.API.Interfaces;
using VVUP.FreeCustomRoles.FreeCustomRoles;

namespace VVUP.FreeCustomRoles
{
    public class MasterConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        
        [Description("These custom roles are for the server owner to make some custom roles themselves, there will be no support for any custom roles that you make with these")]
        public List<FreeCustomRole1> FreeCustomRoles1 { get; set; } = new()
        {
            new FreeCustomRole1(),
        };
        public List<FreeCustomRole2> FreeCustomRoles2 { get; set; } = new()
        {
            new FreeCustomRole2(),
        };
        public List<FreeCustomRole3> FreeCustomRoles3 { get; set; } = new()
        {
            new FreeCustomRole3(),
        };
        public List<FreeCustomRole4> FreeCustomRoles4 { get; set; } = new()
        {
            new FreeCustomRole4(),
        };
        public List<FreeCustomRole5> FreeCustomRoles5 { get; set; } = new()
        {
            new FreeCustomRole5(),
        };
        public List<FreeCustomRole6> FreeCustomRoles6 { get; set; } = new()
        {
            new FreeCustomRole6(),
        };
        public List<FreeCustomRole7> FreeCustomRoles7 { get; set; } = new()
        {
            new FreeCustomRole7(),
        };
        public List<FreeCustomRole8> FreeCustomRoles8 { get; set; } = new()
        {
            new FreeCustomRole8(),
        };
        public List<FreeCustomRole9> FreeCustomRoles9 { get; set; } = new()
        {
            new FreeCustomRole9(),
        };
        public List<FreeCustomRole10> FreeCustomRoles10 { get; set; } = new()
        {
            new FreeCustomRole10(),
        };
        public List<FreeCustomRole11> FreeCustomRoles11 { get; set; } = new()
        {
            new FreeCustomRole11(),
        };
        public List<FreeCustomRole12> FreeCustomRoles12 { get; set; } = new()
        {
            new FreeCustomRole12(),
        };
        public List<FreeCustomRole13> FreeCustomRoles13 { get; set; } = new()
        {
            new FreeCustomRole13(),
        };
        public List<FreeCustomRole14> FreeCustomRoles14 { get; set; } = new()
        {
            new FreeCustomRole14(),
        };
        public List<FreeCustomRole15> FreeCustomRoles15 { get; set; } = new()
        {
            new FreeCustomRole15(),
        };
        public List<FreeCustomRole16> FreeCustomRoles16 { get; set; } = new()
        {
            new FreeCustomRole16(),
        };
        public List<FreeCustomRole17> FreeCustomRoles17 { get; set; } = new()
        {
            new FreeCustomRole17(),
        };
        public List<FreeCustomRole18> FreeCustomRoles18 { get; set; } = new()
        {
            new FreeCustomRole18(),
        };
        public List<FreeCustomRole19> FreeCustomRoles19 { get; set; } = new()
        {
            new FreeCustomRole19(),
        };
        public List<FreeCustomRole20> FreeCustomRoles20 { get; set; } = new()
        {
            new FreeCustomRole20(),
        };
        public string Header { get; set; } = "Vicious Vikki's Custom Roles";
        public int FreeCustomRoleTextId { get; set; } = 3;
    }
}