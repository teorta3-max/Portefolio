// tout le javaScript ici

// Navigation smooth scroll
document.querySelectorAll("nav a").forEach(link=>{
  link.onclick = e=>{
    e.preventDefault();
    const menu = document.getElementById("mobile-menu");
    if (menu.classList.contains("active")) {
      menu.classList.remove("active");
    }
    document.querySelector(link.getAttribute("href"))
      .scrollIntoView({behavior:"smooth"});
  };
});

// Menu mobile toggle
const burger = document.getElementById("burger");
const menu = document.getElementById("mobile-menu");

burger.onclick = () => {
  menu.classList.toggle("active");
};

// Close menu when clicking outside
document.addEventListener("click", (e) => {
  if (!e.target.closest(".barredenavigation")) {
    menu.classList.remove("active");
  }
});

// Form submission
document.querySelector("form").addEventListener("submit", () => {
  alert(" Message envoyé ! Merci de m'avoir contacté.");
});

// Skills animation
const skills = document.querySelectorAll(".skill-item");
const observer = new IntersectionObserver((entries) => {
  entries.forEach(entry => {
    if (entry.isIntersecting) {
      entry.target.classList.add("visible");
      observer.unobserve(entry.target);
    }
  });
}, { threshold: 0.1 });

skills.forEach(skill => observer.observe(skill));

// Compétences animation
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