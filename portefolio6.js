// tout le javaScript ici
document.querySelectorAll("nav a").forEach(link=>{
  link.onclick = e=>{
    e.preventDefault();
    document.querySelector(link.getAttribute("href"))
      .scrollIntoView({behavior:"smooth"});
  };
});

document.querySelector("form").addEventListener("submit", () => {
  alert(" Message envoyé ! Merci de m'avoir contacté.");
});

const burger = document.getElementById("burger");
const menu = document.getElementById("mobile-menu");

burger.onclick = () => {
  menu.style.display = menu.style.display === "flex" ? "none" : "flex";
};

const skills = document.querySelectorAll(".skill-item");

const observer = new IntersectionObserver((entries) => {
  entries.forEach(entry => {
    if (entry.isIntersecting) {
      entry.target.classList.add("visible");
      observer.unobserve(entry.target);
    }
  });
}, { threshold: 0.1 });

const competences = document.querySelectorAll(".unecompétence");
const compObserver = new IntersectionObserver((entries) => {
  entries.forEach(entry => {
    if (entry.isIntersecting) {
      entry.target.classList.add("visible");
      compObserver.unobserve(entry.target);
    }
  });
}, { threshold: 0.1 });

competences.forEach(c => compObserver.observe(c));