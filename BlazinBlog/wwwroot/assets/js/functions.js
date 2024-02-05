document.addEventListener('DOMContentLoaded', function () {
    const topNav = document.getElementsByClassName('topnav')[0];
    if (topNav) {
        window.addEventListener('scroll', function () {
            topNav.classList.toggle('scrollednav', window.scrollY > 50);
            topNav.classList.toggle('py-0', window.scrollY > 50);
        });
     }
});


function toggleMenu(e) {
    e.target.classList.toggle('collapsed');
    document.getElementById("top-navbar-menu-wrapper").classList.toggle("show");
}