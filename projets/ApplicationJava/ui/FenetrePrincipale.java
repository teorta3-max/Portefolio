package ui;

import dao.EtudiantDAO;
import model.Etudiant;

import javax.swing.*;
import javax.swing.table.DefaultTableModel;
import java.awt.*;
import java.awt.event.*;

public class FenetrePrincipale extends JFrame {
    private EtudiantDAO dao = new EtudiantDAO();
    private JTable table;
    private DefaultTableModel model;

    private JTextField txtNom, txtAge, txtMoyenne;
    private int selectedId = -1;

    public FenetrePrincipale() {
        setTitle("Gestion des Étudiants");
        setSize(700, 400);
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setLocationRelativeTo(null);
        setLayout(new BorderLayout(10, 10));

        // --- PANNEAU FORMULAIRE ---
        JPanel panelForm = new JPanel(new GridLayout(4, 2, 10, 10));
        panelForm.setBorder(BorderFactory.createTitledBorder("Informations Étudiant"));

        txtNom = new JTextField();
        txtAge = new JTextField();
        txtMoyenne = new JTextField();

        panelForm.add(new JLabel("Nom :"));
        panelForm.add(txtNom);
        panelForm.add(new JLabel("Âge :"));
        panelForm.add(txtAge);
        panelForm.add(new JLabel("Moyenne :"));
        panelForm.add(txtMoyenne);

        JButton btnReset = new JButton("Réinitialiser");
        btnReset.addActionListener(e -> viderChamps());
        panelForm.add(btnReset);

        // --- PANNEAU TABLEAU ---
        model = new DefaultTableModel(new Object[]{"ID", "Nom", "Âge", "Moyenne"}, 0);
        table = new JTable(model);
        table.setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
        table.getSelectionModel().addListSelectionListener(e -> remplirChampsDepuisTable());

        JScrollPane scrollPane = new JScrollPane(table);

        // --- PANNEAU BOUTONS ---
        JPanel panelBoutons = new JPanel(new FlowLayout());
        JButton btnAjouter = new JButton("Ajouter");
        JButton btnModifier = new JButton("Modifier");
        JButton btnSupprimer = new JButton("Supprimer");

        btnAjouter.addActionListener(e -> ajouterEtudiant());
        btnModifier.addActionListener(e -> modifierEtudiant());
        btnSupprimer.addActionListener(e -> supprimerEtudiant());

        panelBoutons.add(btnAjouter);
        panelBoutons.add(btnModifier);
        panelBoutons.add(btnSupprimer);

        add(panelForm, BorderLayout.WEST);
        add(scrollPane, BorderLayout.CENTER);
        add(panelBoutons, BorderLayout.SOUTH);
    }

    private void ajouterEtudiant() {
        try {
            String nom = txtNom.getText();
            int age = Integer.parseInt(txtAge.getText());
            double moyenne = Double.parseDouble(txtMoyenne.getText());
            dao.ajouterEtudiant(nom, age, moyenne);
            rafraichirTable();
            viderChamps();
        } catch (Exception ex) {
            JOptionPane.showMessageDialog(this, "Entrée invalide !");
        }
    }

    private void modifierEtudiant() {
        if (selectedId == -1) return;
        try {
            String nom = txtNom.getText();
            int age = Integer.parseInt(txtAge.getText());
            double moyenne = Double.parseDouble(txtMoyenne.getText());
            dao.modifierEtudiant(selectedId, nom, age, moyenne);
            rafraichirTable();
            viderChamps();
        } catch (Exception ex) {
            JOptionPane.showMessageDialog(this, "Entrée invalide !");
        }
    }

    private void supprimerEtudiant() {
        if (selectedId == -1) return;
        dao.supprimerEtudiant(selectedId);
        rafraichirTable();
        viderChamps();
    }

    private void rafraichirTable() {
        model.setRowCount(0);
        for (Etudiant e : dao.getEtudiants()) {
            model.addRow(new Object[]{e.getId(), e.getNom(), e.getAge(), e.getMoyenne()});
        }
    }

    private void remplirChampsDepuisTable() {
        int row = table.getSelectedRow();
        if (row == -1) return;
        selectedId = (int) model.getValueAt(row, 0);
        txtNom.setText((String) model.getValueAt(row, 1));
        txtAge.setText(model.getValueAt(row, 2).toString());
        txtMoyenne.setText(model.getValueAt(row, 3).toString());
    }

    private void viderChamps() {
        selectedId = -1;
        txtNom.setText("");
        txtAge.setText("");
        txtMoyenne.setText("");
        table.clearSelection();
    }
}