package dao;

import java.util.ArrayList;
import java.util.List;
import model.Etudiant;

public class EtudiantDAO {
    private List<Etudiant> etudiants = new ArrayList<>();
    private int idCounter = 1;

    public void ajouterEtudiant(String nom, int age, double moyenne) {
        etudiants.add(new Etudiant(idCounter++, nom, age, moyenne));
    }

    public void modifierEtudiant(int id, String nom, int age, double moyenne) {
        for (Etudiant e : etudiants) {
            if (e.getId() == id) {
                e.setNom(nom);
                e.setAge(age);
                e.setMoyenne(moyenne);
                break;
            }
        }
    }

    public void supprimerEtudiant(int id) {
        etudiants.removeIf(e -> e.getId() == id);
    }

    public List<Etudiant> getEtudiants() {
        return etudiants;
    }
}