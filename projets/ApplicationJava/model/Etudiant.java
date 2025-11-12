package model;

public class Etudiant {
    private int id;
    private String nom;
    private int age;
    private double moyenne;

    public Etudiant(int id, String nom, int age, double moyenne) {
        this.id = id;
        this.nom = nom;
        this.age = age;
        this.moyenne = moyenne;
    }

    public int getId() { return id; }
    public String getNom() { return nom; }
    public int getAge() { return age; }
    public double getMoyenne() { return moyenne; }

    public void setNom(String nom) { this.nom = nom; }
    public void setAge(int age) { this.age = age; }
    public void setMoyenne(double moyenne) { this.moyenne = moyenne; }
}