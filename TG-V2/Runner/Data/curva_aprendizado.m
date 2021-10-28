function curva_aprendizado()
    data = load('curva_aprendizado.csv');

    x = data(:, 1);

    hold on;

    plot(x, data(:, 2));
    plot(x, data(:, 3));
    plot(x, data(:, 4));
    plot(x, data(:, 5));

    grid on;
    title("Curva de aprendizado (algoritmo genético)");
    xlabel("Geração");
    ylabel("Ganho normalizado");

    # 52, 0.5f
    # 127 0.4f
    # 177, 0.6f
    # 227, 0.1f

    legend('P: 52, Mp: 50%', 'P: 127, Mp: 40%', 'P: 177, Mp: 60%', 'P: 227, Mp: 10%');
    pause();
endfunction